using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using FluentFTP;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO;
using System.Net;

namespace Shared;

public static class FileStorageExtensions
{
    public static void AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration["FileStorage:Provider"] == FileStorageProvider.S3)
        {
            services.AddAWSService<IAmazonS3>(new AWSOptions
            {
                Credentials = new BasicAWSCredentials(configuration["FileStorage:S3:AccessKey"], configuration["FileStorage:S3:AccessSecret"]),
                Region = RegionEndpoint.USEast1, // need to replace
            });
            services.AddSingleton<IFileStorage, AwsS3FileStorage>();
        }

        if (configuration["FileStorage:Provider"] == FileStorageProvider.Local)
        {
            services.AddSingleton<IFileStorage, LocalFileStorage>();
        }

        if (configuration["FileStorage:Provider"] == FileStorageProvider.FTP)
        {
            services.AddSingleton<IFileStorage>(p => new FTPFileStorage
            {
                Host = string.Concat(configuration["FileStorage:FTP:Host"]),
                UserName = configuration["FileStorage:FTP:UserName"],
                Password = configuration["FileStorage:FTP:Password"],
                Secure = configuration["FileStorage:FTP:Secure"].Equals("true", StringComparison.CurrentCultureIgnoreCase),
            });
        }

        if (configuration["FileStorage:Provider"] == FileStorageProvider.SFTP)
        {
            services.AddSingleton<IFileStorage>(p => new SFTPFileStorage
            {
                Host = configuration["FileStorage:SFTP:Host"],
                Port = configuration["FileStorage:SFTP:Port"].ToInt32(),
                UserName = configuration["FileStorage:SFTP:UserName"],
                Password = configuration["FileStorage:SFTP:Password"],
                BaseDirectory = configuration["FileStorage:SFTP:BaseDirectory"],
            });
        }

        if (configuration["FileStorage:Provider"] == FileStorageProvider.Google)
        {
            services.AddSingleton<IFileStorage>(p => new GoogleFileStorage(new GoogleConfig
            {
                CredentialFile = configuration["FileStorage:Google:CredentialFile"],
                BucketName = configuration["FileStorage:Google:BucketName"],
                ProjectId = configuration["FileStorage:Google:ProjectId"],
            }));
        }
    }
}

public class FileStorageProvider
{
    public const string S3 = "S3";
    public const string Google = "GOOGLE";
    public const string Local = "LOCAL";
    public const string FTP = "FTP";
    public const string SFTP = "SFTP";
}

public interface IFileStorage
{
    void SaveTo(string path, IFormFile file);
    void SaveTo(string path, Stream stream);
    Stream GetFrom(string path);
    void RemoveFrom(string path);

    void SaveToImports(string key, IFormFile file);
    void SaveToImports(string key, Stream stream);
    Stream GetFromImports(string key);
    void RemoveFromImports(string key);
    bool ImportsExists(string key);

    void SaveToExports(string key, IFormFile file);
    void SaveToExports(string key, Stream stream);
    Stream GetFromExports(string key);
    void RemoveFromExports(string key);
    bool ExportsExists(string key);

    void SaveToAttachments(string key, IFormFile file);
    void SaveToAttachments(string key, Stream stream);
    Stream GetFromAttachments(string key);
    void RemoveFromAttachments(string key);
    bool AttachmentsExists(string key);

    void SaveToImages(string key, IFormFile file);
    void SaveToImages(string key, Stream stream);
    Stream GetFromImages(string key);
    Stream GetFromImagesThumbnail(string key);
    void RemoveFromImages(string key);
    bool ImagesExists(string key);


    void Save(string documentType, string key, IFormFile file, DateTime date);
    void Save(string documentType, string key, Stream stream, DateTime date);
    Stream Get(string documentType, string key, DateTime date);
    void Remove(string documentType, string key, DateTime date);
    bool Exists(string documentType, string key, DateTime date);

    //void RemoveFromImagesThumbnail(string key);
}

public class LocalFileStorage : IFileStorage
{
    protected IConfiguration Configuration;
    protected string BaseDirectory;

    public LocalFileStorage(IConfiguration configuration)
    {
        Configuration = configuration;
        BaseDirectory = Configuration["FileStorage:LocalStoragePath"];
    }

    public bool AttachmentsExists(string key)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return File.Exists(filePath);
    }

    public bool Exists(string documentType, string key, DateTime date)
    {
        throw new NotImplementedException();
    }

    public bool ExportsExists(string key)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return File.Exists(filePath);
    }

    public Stream Get(string documentType, string key, DateTime date)
    {
        throw new NotImplementedException();
    }

    public Stream GetFrom(string path)
    {
        throw new NotImplementedException();
    }

    public Stream GetFromAttachments(string id)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        return new FileStream(filePath, FileMode.Open, FileAccess.Read);
    }

    public Stream GetFromExports(string id)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var fileStream = new FileStream(filePath, FileMode.Open);
        return fileStream;
    }

    public Stream GetFromImages(string id)
    {
        var location = Path.Combine(BaseDirectory, "images");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var fileStream = new FileStream(filePath, FileMode.Open);
        return fileStream;
    }

    public Stream GetFromImagesThumbnail(string key)
    {
        throw new NotImplementedException();
    }

    public Stream GetFromImagesThumnail(string id)
    {
        var location = Path.Combine(BaseDirectory, "images", "thumbnail");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var fileStream = new FileStream(filePath, FileMode.Open);
        return fileStream;
    }

    public Stream GetFromImports(string id)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var fileStream = new FileStream(filePath, FileMode.Open);
        return fileStream;
    }

    public bool ImagesExists(string key)
    {
        var location = Path.Combine(BaseDirectory, "images");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return File.Exists(filePath);
    }

    public bool ImportsExists(string key)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return File.Exists(filePath);
    }

    public void Remove(string documentType, string key, DateTime date)
    {
        throw new NotImplementedException();
    }

    public void RemoveFrom(string path)
    {
        throw new NotImplementedException();
    }

    public void RemoveFromAttachments(string key)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    public void RemoveFromExports(string key)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    public void RemoveFromImages(string key)
    {
        var location = Path.Combine(BaseDirectory, "images");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    public void RemoveFromImports(string key)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    public void Save(string documentType, string key, IFormFile file, DateTime date)
    {
        throw new NotImplementedException();
    }

    public void Save(string documentType, string key, Stream stream, DateTime date)
    {
        throw new NotImplementedException();
    }

    public void SaveTo(string path, IFormFile file)
    {
        throw new NotImplementedException();
    }

    public void SaveTo(string path, Stream stream)
    {
        throw new NotImplementedException();
    }

    public void SaveToAttachments(string key, IFormFile file)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        var stream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(stream);
        stream.Flush();
        stream.Dispose();
        stream.Close();
    }

    public void SaveToAttachments(string key, Stream stream)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        using var fileStream = new FileStream(filePath, FileMode.Create);
        stream.CopyTo(fileStream);

        fileStream.Dispose();
        stream.Dispose();
    }

    public void SaveToExports(string key, IFormFile file)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        var stream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(stream);

        stream.Dispose();
    }

    public void SaveToExports(string key, Stream stream)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        using var fileStream = new FileStream(filePath, FileMode.Create);
        stream.CopyTo(fileStream);
        fileStream.Dispose();
    }

    public void SaveToImages(string key, IFormFile file)
    {
        var location = Path.Combine(BaseDirectory, "images");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        using var stream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(stream);
        stream.Dispose();
    }

    public void SaveToImages(string key, Stream stream)
    {
        var location = Path.Combine(BaseDirectory, "images");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        using var fileStream = new FileStream(filePath, FileMode.Create);
        stream.CopyTo(fileStream);
        fileStream.Dispose();
    }

    public void SaveToImports(string key, IFormFile file)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        using var stream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(stream);
        stream.Dispose();
    }

    public void SaveToImports(string key, Stream stream)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        using var fileStream = new FileStream(filePath, FileMode.Create);
        stream.CopyTo(fileStream);
        fileStream.Dispose();
    }
}

public class GoogleFileStorage : IFileStorage
{

    protected string BucketName { get; }
    protected StorageClient Client { get; }
    protected GoogleConfig Config { get; }

    //protected IConfiguration Configuration { get; }

    public GoogleFileStorage(GoogleConfig config)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        var credential = GoogleCredential.FromFile(config.CredentialFile);
#pragma warning restore CS0618 // Type or member is obsolete
        Client = StorageClient.Create(credential);
        BucketName = config.BucketName;
        Config = config;
        //Configuration = configuration;
    }

    public async Task CreateBucket(string name)
    {
        try
        {
            var asd = await Client.GetBucketAsync(BucketName);
            if (asd != null)
                throw new Exception("The bucket already exists");

            var bucket = await Client.CreateBucketAsync(Config.ProjectId, name);
        }
        catch
        {
            throw;
        }
    }

    public async Task DeleteBucket(string name)
    {

        try
        {
            var asd = await Client.GetBucketAsync(BucketName);
            if (asd == null)
                throw new Exception("The bucket does not exists");

            await Client.DeleteBucketAsync(name);
        }
        catch
        {
            throw;
        }
    }

    public async Task AddFileToBucket(string key, Stream file)
    {
        await AddFileToBucket(key, file, BucketName);
    }

    public async Task AddFileToBucket(string key, Stream file, string bucketName)
    {
        await Client.UploadObjectAsync(bucketName, key, null, file);
    }

    protected async Task DeleteFileFromBucket(string key)
    {
        await DeleteFileFromBucket(key, BucketName);
    }

    protected async Task DeleteFileFromBucket(string key, string bucketName)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        await Client.DeleteObjectAsync(bucketName, key);
    }

    public Stream GetFromAttachments(string key)
    {
        var stream = new MemoryStream();
        var down = Client.DownloadObject(BucketName, $"attachments/{key.Sha256Hex()}", stream);
        return stream;
    }

    public Stream GetFromExports(string key)
    {
        var stream = new MemoryStream();
        var down = Client.DownloadObjectAsync(BucketName, $"exports/{key.Sha256Hex()}", stream).Result;
        return stream;
    }

    public Stream GetFromImages(string key)
    {
        var stream = new MemoryStream();
        var down = Client.DownloadObjectAsync(BucketName, $"images/{key.Sha256Hex()}", stream).Result;
        return stream;
    }

    public Stream GetFromImagesThumnail(string key)
    {
        var stream = new MemoryStream();
        var down = Client.DownloadObjectAsync(BucketName, $"images/thumbnails/{key.Sha256Hex()}", stream).Result;
        return stream;
    }

    public Stream GetFromImports(string key)
    {
        var stream = new MemoryStream();
        var down = Client.DownloadObjectAsync(BucketName, $"imports/{key.Sha256Hex()}", stream).Result;
        return stream;
    }

    public void SaveToAttachments(string key, IFormFile file)
    {
        AddFileToBucket($"attachments/{key.Sha256Hex()}", file.OpenReadStream()).Wait();
    }

    public void SaveToAttachments(string key, Stream stream)
    {
        AddFileToBucket($"attachments/{key.Sha256Hex()}", stream).Wait();
    }

    public void SaveToExports(string key, IFormFile file)
    {
        AddFileToBucket($"exports/{key.Sha256Hex()}", file.OpenReadStream()).Wait();
    }

    public void SaveToExports(string key, Stream stream)
    {
        AddFileToBucket($"exports/{key.Sha256Hex()}", stream).Wait();
    }

    public void SaveToImages(string key, IFormFile file)
    {
        AddFileToBucket($"images/{key.Sha256Hex()}", file.OpenReadStream()).Wait();
    }

    public void SaveToImages(string key, Stream stream)
    {
        AddFileToBucket($"images/{key.Sha256Hex()}", stream).Wait();
    }

    public void SaveToImports(string key, IFormFile file)
    {
        AddFileToBucket($"imports/{key.Sha256Hex()}", file.OpenReadStream()).Wait();
    }

    public void SaveToImports(string key, Stream stream)
    {
        AddFileToBucket($"imports/{key.Sha256Hex()}", stream).Wait();
    }

    public void RemoveFromImports(string key)
    {
        DeleteFileFromBucket($"imports/{key.Sha256Hex()}").Wait();
    }

    public void RemoveFromExports(string key)
    {
        DeleteFileFromBucket($"exports/{key.Sha256Hex()}").Wait();
    }

    public void RemoveFromAttachments(string key)
    {
        DeleteFileFromBucket($"attachments/{key.Sha256Hex()}").Wait();
    }

    public Stream GetFromImagesThumbnail(string key)
    {
        throw new NotImplementedException();
    }

    public void RemoveFromImages(string key)
    {
        DeleteFileFromBucket($"images/{key.Sha256Hex()}").Wait();
    }

    public void RemoveFromImagesThumbnail(string key)
    {
        throw new NotImplementedException();
    }

    public bool ImportsExists(string key)
    {
        throw new NotImplementedException();
    }

    public bool ExportsExists(string key)
    {
        throw new NotImplementedException();
    }

    public bool AttachmentsExists(string key)
    {
        throw new NotImplementedException();
    }

    public bool ImagesExists(string key)
    {
        throw new NotImplementedException();
    }

    public void SaveTo(string path, IFormFile file)
    {
        throw new NotImplementedException();
    }

    public void SaveTo(string path, Stream stream)
    {
        throw new NotImplementedException();
    }

    public Stream GetFrom(string path)
    {
        throw new NotImplementedException();
    }

    public void RemoveFrom(string path)
    {
        throw new NotImplementedException();
    }

    public void Save(string documentType, string key, IFormFile file, DateTime date)
    {
        throw new NotImplementedException();
    }

    public void Save(string documentType, string key, Stream stream, DateTime date)
    {
        throw new NotImplementedException();
    }

    public Stream Get(string documentType, string key, DateTime date)
    {
        throw new NotImplementedException();
    }

    public void Remove(string documentType, string key, DateTime date)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string documentType, string key, DateTime date)
    {
        throw new NotImplementedException();
    }
}

public class AwsS3FileStorage : IFileStorage
{
    protected string BucketName { get; }
    protected IAmazonS3 Client { get; }
    protected IConfiguration Configuration { get; }

    public AwsS3FileStorage(IAmazonS3 client, IConfiguration configuration)
    {
        BucketName = configuration["FileStorage:S3:BucketName"];
        Client = client;
        Configuration = configuration;
    }

    public async Task<AwsS3Response> CreateBucket(string name)
    {
        try
        {
            var existBucket = await AmazonS3Util.DoesS3BucketExistV2Async(Client, Configuration["AWS:S3:BucketPrefix"] + name);

            if (existBucket)
                return new AwsS3Response { Message = "The bucket already exists", Status = HttpStatusCode.Conflict };

            var putBucketRequest = new PutBucketRequest
            {
                BucketName = Configuration["AWS:S3:BucketPrefix"] + name,
                UseClientRegion = true,
            };

            var response = await Client.PutBucketAsync(putBucketRequest);

            return new AwsS3Response { Message = $"Bucket with name {Configuration["AWS:S3:BucketPrefix"] + name} was successfully created", Status = response.HttpStatusCode };
        }
        catch (AmazonS3Exception e)
        {
            return new AwsS3Response
            {
                Message = e.Message,
                Status = e.StatusCode
            };
        }
        catch (Exception e)
        {
            return new AwsS3Response
            {
                Message = e.Message,
                Status = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<AwsS3Response> DeleteBucket(string name)
    {
        try
        {
            var existBucket = await AmazonS3Util.DoesS3BucketExistV2Async(Client, Configuration["AWS:S3:BucketPrefix"] + name);

            if (!existBucket)
                return new AwsS3Response { Message = $"Bucket with name {Configuration["AWS:S3:BucketPrefix"] + name} was not found", Status = HttpStatusCode.NotFound };

            var response = await Client.DeleteBucketAsync(Configuration["AWS:S3:BucketPrefix"] + name);

            return new AwsS3Response { Message = response.ResponseMetadata.RequestId, Status = response.HttpStatusCode };
        }
        catch (AmazonS3Exception e)
        {
            return new AwsS3Response { Message = e.Message, Status = e.StatusCode };
        }
        catch (Exception e)
        {
            return new AwsS3Response { Message = e.Message, Status = HttpStatusCode.InternalServerError };
        }
    }
    protected async Task<AwsS3Response> AddFileToBucket(string fileNames)
    {
        return await AddFileToBucket(fileNames, BucketName);
    }

    protected async Task<AwsS3Response> AddFileToBucket(string fileNames, string bucketName)
    {
        try
        {
            var fileTransferUtility = new TransferUtility(Client);
            await fileTransferUtility.UploadAsync(fileNames.Sha256Hex(), bucketName);
        }
        catch (AmazonS3Exception e)
        {
            return new AwsS3Response { Message = e.Message, Status = e.StatusCode };
        }
        catch (Exception e)
        {
            return new AwsS3Response { Message = e.Message, Status = HttpStatusCode.InternalServerError };
        }

        return new AwsS3Response { Message = "File uploaded Successfully", Status = HttpStatusCode.OK };
    }

    protected async Task<AwsS3Response> AddFileToBucket(string key, Stream file)
    {
        return await AddFileToBucket(key, file, BucketName);
    }

    protected async Task<AwsS3Response> AddFileToBucket(string key, Stream file, string bucketName)
    {
        try
        {
            var fileTransferUtility = new TransferUtility(Client);

            // Option 1 (Upload an existing file in your computer to the S3)
            await fileTransferUtility.UploadAsync(file, bucketName, key);
        }
        catch (AmazonS3Exception e)
        {
            return new AwsS3Response { Message = e.Message, Status = e.StatusCode };
        }
        catch (Exception e)
        {
            return new AwsS3Response { Message = e.Message, Status = HttpStatusCode.InternalServerError };
        }

        return new AwsS3Response { Message = "File uploaded Successfully", Status = HttpStatusCode.OK };
    }

    protected async Task<AwsS3Response> DeleteFileFromBucket(string key)
    {
        return await DeleteFileFromBucket(key, BucketName);
    }

    protected async Task<AwsS3Response> DeleteFileFromBucket(string key, string bucketName)
    {
        if (string.IsNullOrEmpty(key))
            key = "test.txt";

        try
        {
            var request = new GetObjectRequest { BucketName = bucketName, Key = key };

            var response = await Client.GetObjectAsync(request);

            if (response == null || response.HttpStatusCode != HttpStatusCode.OK)
                return new AwsS3Response { Message = "Error getting the object from the bucket", Status = HttpStatusCode.NotFound };

            await Client.DeleteObjectAsync(bucketName, key);

            return new AwsS3Response { Message = "The file was successfully deleted", Status = HttpStatusCode.OK };
        }
        catch (AmazonS3Exception e)
        {
            return new AwsS3Response { Message = e.Message, Status = e.StatusCode };
        }
        catch (Exception e)
        {
            return new AwsS3Response { Message = e.Message, Status = HttpStatusCode.InternalServerError };
        }
    }

    protected string GetSignedUrl(string fileName, TimeSpan expiresIn, string method = "GET")
    {
        return GetSignedUrl(fileName, BucketName, expiresIn, method);
    }

    protected string GetSignedUrl(string key, string bucketName, TimeSpan expiresIn, string method = "GET")
    {
        var urlRequest = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddSeconds(expiresIn.TotalSeconds),
            Verb = method.ToUpper() == "GET" ? HttpVerb.GET : HttpVerb.PUT,
            //ResponseHeaderOverrides = headers,
        };

        return Client.GetPreSignedURL(urlRequest);
    }

    public Stream GetFromAttachments(string key)
    {
        return Client.GetObjectAsync(BucketName, $"attachments/{key.Sha256Hex()}").Result.ResponseStream;
    }

    public Stream GetFromExports(string key)
    {
        return Client.GetObjectAsync(BucketName, $"exports/{key.Sha256Hex()}").Result.ResponseStream;
    }

    public Stream GetFromImages(string key)
    {
        return Client.GetObjectAsync(BucketName, $"images/{key.Sha256Hex()}").Result.ResponseStream;
    }

    public Stream GetFromImagesThumnail(string key)
    {
        return Client.GetObjectAsync(BucketName, $"images/thumbnails/{key.Sha256Hex()}").Result.ResponseStream;
    }

    public Stream GetFromImports(string key)
    {
        return Client.GetObjectAsync(BucketName, $"imports/{key.Sha256Hex()}").Result.ResponseStream;
    }

    public void SaveToAttachments(string key, IFormFile file)
    {
        AddFileToBucket($"attachments/{key.Sha256Hex()}", file.OpenReadStream()).Wait();
    }

    public void SaveToAttachments(string key, Stream stream)
    {
        AddFileToBucket($"attachments/{key.Sha256Hex()}", stream).Wait();
    }

    public void SaveToExports(string key, IFormFile file)
    {
        AddFileToBucket($"exports/{key.Sha256Hex()}", file.OpenReadStream()).Wait();
    }

    public void SaveToExports(string key, Stream stream)
    {
        AddFileToBucket($"exports/{key.Sha256Hex()}", stream).Wait();
    }

    public void SaveToImages(string key, IFormFile file)
    {
        AddFileToBucket($"images/{key.Sha256Hex()}", file.OpenReadStream()).Wait();
    }

    public void SaveToImages(string key, Stream stream)
    {
        AddFileToBucket($"images/{key.Sha256Hex()}", stream).Wait();
    }

    public void SaveToImports(string key, IFormFile file)
    {
        AddFileToBucket($"imports/{key.Sha256Hex()}", file.OpenReadStream()).Wait();
    }

    public void SaveToImports(string key, Stream stream)
    {
        AddFileToBucket($"imports/{key.Sha256Hex()}", stream).Wait();
    }

    public void RemoveFromImports(string key)
    {
        DeleteFileFromBucket($"imports/{key.Sha256Hex()}").Wait();
    }

    public void RemoveFromExports(string key)
    {
        DeleteFileFromBucket($"exports/{key.Sha256Hex()}").Wait();
    }

    public void RemoveFromAttachments(string key)
    {
        DeleteFileFromBucket($"attachments/{key.Sha256Hex()}").Wait();
    }

    public Stream GetFromImagesThumbnail(string key)
    {
        throw new NotImplementedException();
    }

    public void RemoveFromImages(string key)
    {
        DeleteFileFromBucket($"images/{key.Sha256Hex()}").Wait();
    }

    public void RemoveFromImagesThumbnail(string key)
    {
        throw new NotImplementedException();
    }

    public bool ImportsExists(string key)
    {
        throw new NotImplementedException();
    }

    public bool ExportsExists(string key)
    {
        throw new NotImplementedException();
    }

    public bool AttachmentsExists(string key)
    {
        throw new NotImplementedException();
    }

    public bool ImagesExists(string key)
    {
        throw new NotImplementedException();
    }

    public void SaveTo(string path, IFormFile file)
    {
        throw new NotImplementedException();
    }

    public void SaveTo(string path, Stream stream)
    {
        throw new NotImplementedException();
    }

    public Stream GetFrom(string path)
    {
        throw new NotImplementedException();
    }

    public void RemoveFrom(string path)
    {
        throw new NotImplementedException();
    }

    public void Save(string documentType, string key, IFormFile file, DateTime date)
    {
        throw new NotImplementedException();
    }

    public void Save(string documentType, string key, Stream stream, DateTime date)
    {
        throw new NotImplementedException();
    }

    public Stream Get(string documentType, string key, DateTime date)
    {
        throw new NotImplementedException();
    }

    public void Remove(string documentType, string key, DateTime date)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string documentType, string key, DateTime date)
    {
        throw new NotImplementedException();
    }
}

public class FTPFileStorage : IFileStorage
{
    public string Host { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool Secure { get; set; }
    public string BaseDirectory { get; set; }

    // Membuat koneksi FTP
    private FtpClient CreateFtpClient()
    {
        var client = new FtpClient(Host, UserName, Password);
        if (Secure)
        {
            client.Config.EncryptionMode = FtpEncryptionMode.Explicit;
            client.ValidateCertificate += (control, e) =>
            {
                e.Accept = true; // Terima semua sertifikat (gunakan dengan hati-hati)
            };
        }
        return client;
    }

    private bool UploadFile(string localFilePath, string remoteFilePath)
    {
        using var client = CreateFtpClient();
        client.Connect();

        return client.UploadFile(localFilePath, remoteFilePath) == FtpStatus.Success;
    }

    private bool UploadFile(Stream stream, string remoteFilePath)
    {
        using var client = CreateFtpClient();
        client.Connect();

        return client.UploadStream(stream, remoteFilePath) == FtpStatus.Success;
    }

    private bool DownloadFile(string remoteFilePath, string localFilePath)
    {
        using var client = CreateFtpClient();
        client.Connect();
        return client.DownloadFile(localFilePath, remoteFilePath) == FtpStatus.Success;
    }

    private bool DownloadFile(Stream stream, string remoteFilePath)
    {
        using var client = CreateFtpClient();
        client.Connect();
        return client.DownloadStream(stream, remoteFilePath);
    }

    private void DeleteFile(string remoteFilePath)
    {
        using var client = CreateFtpClient();
        client.Connect();
        client.DeleteFile(remoteFilePath);
    }

    private bool FileExists(string remoteFilePath)
    {
        using var client = CreateFtpClient();
        client.Connect();
        return client.FileExists(remoteFilePath);
    }

    private bool DirectoryExists(string remoteFilePath)
    {
        using var client = CreateFtpClient();
        client.Connect();
        return client.DirectoryExists(remoteFilePath);
    }

    public FtpListItem GetInfo(string remoteFilePath)
    {
        using var client = CreateFtpClient();
        client.Connect();
        return client.GetObjectInfo(remoteFilePath);
    }

    private bool CreateDirectory(string remoteFilePath)
    {
        using var client = CreateFtpClient();
        client.Connect();
        return client.CreateDirectory(remoteFilePath);
    }

    private IEnumerable<string> ListFiles(string remoteDirectory)
    {
        using var client = CreateFtpClient();
        client.Connect();
        return client.GetListing(remoteDirectory).Select(p => p.FullName);
    }


    public bool AttachmentsExists(string key)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return FileExists(filePath);
    }

    public bool ExportsExists(string key)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return FileExists(filePath);
    }

    public Stream GetFromAttachments(string id)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var stream = new MemoryStream();
        DownloadFile(stream, filePath);
        return stream;
    }

    public Stream GetFromExports(string id)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var stream = new MemoryStream();
        DownloadFile(stream, filePath);
        return stream;
    }

    public Stream GetFromImages(string id)
    {
        var location = Path.Combine(BaseDirectory, "images");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var stream = new MemoryStream();
        DownloadFile(stream, filePath);
        return stream;
    }

    public Stream GetFromImagesThumbnail(string key)
    {
        throw new NotImplementedException();
    }

    public Stream GetFromImagesThumnail(string id)
    {
        var location = Path.Combine(BaseDirectory, "images", "thumbnail");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var stream = new MemoryStream();
        DownloadFile(stream, filePath);
        return stream;
    }

    public Stream GetFromImports(string id)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var stream = new MemoryStream();
        DownloadFile(stream, filePath);
        return stream;
    }

    public bool ImagesExists(string key)
    {
        var location = Path.Combine(BaseDirectory, "images");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return FileExists(filePath);
    }

    public bool ImportsExists(string key)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return FileExists(filePath);
    }

    public void RemoveFromAttachments(string key)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (FileExists(filePath))
            DeleteFile(filePath);
    }

    public void RemoveFromExports(string key)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (FileExists(filePath))
            DeleteFile(filePath);
    }

    public void RemoveFromImages(string key)
    {
        var location = Path.Combine(BaseDirectory, "images");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (FileExists(filePath))
            DeleteFile(filePath);
    }

    public void RemoveFromImports(string key)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (FileExists(filePath))
            DeleteFile(filePath);
    }

    public void SaveToAttachments(string key, IFormFile file)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(file.OpenReadStream(), filePath);
    }

    public void SaveToAttachments(string key, Stream stream)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(stream, filePath);
    }

    public void SaveToExports(string key, IFormFile file)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(file.OpenReadStream(), filePath);
    }

    public void SaveToExports(string key, Stream stream)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(stream, filePath);
    }

    public void SaveToImages(string key, IFormFile file)
    {
        var location = Path.Combine(BaseDirectory, "images");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(file.OpenReadStream(), filePath);
    }

    public void SaveToImages(string key, Stream stream)
    {
        var location = Path.Combine(BaseDirectory, "images");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(stream, filePath);
    }

    public void SaveToImports(string key, IFormFile file)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(file.OpenReadStream(), filePath);
    }

    public void SaveToImports(string key, Stream stream)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(stream, filePath);
    }

    public void SaveTo(string path, IFormFile file)
    {
        throw new NotImplementedException();
    }

    public void SaveTo(string path, Stream stream)
    {
        throw new NotImplementedException();
    }

    public Stream GetFrom(string path)
    {
        var ms = new MemoryStream();
        DownloadFile(ms, path);
        return ms;
    }

    public void RemoveFrom(string path)
    {
        throw new NotImplementedException();
    }

    public void Save(string documentType, string key, IFormFile file, DateTime date)
    {
        throw new NotImplementedException();
    }

    public void Save(string documentType, string key, Stream stream, DateTime date)
    {
        throw new NotImplementedException();
    }

    public Stream Get(string documentType, string key, DateTime date)
    {
        throw new NotImplementedException();
    }

    public void Remove(string documentType, string key, DateTime date)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string documentType, string key, DateTime date)
    {
        throw new NotImplementedException();
    }
}

public class SFTPFileStorage : IFileStorage
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string BaseDirectory { get; set; }

    // Membuat koneksi FTP
    private SftpClient CreateSFtpClient()
    {
        var client = new SftpClient(Host, Port, UserName, Password);
        return client;
    }

    private void UploadFile(string localFilePath, string remoteFilePath)
    {
        using var client = CreateSFtpClient();
        client.Connect();
        var fs = File.OpenRead(localFilePath);
        client.UploadFile(fs, remoteFilePath);
    }

    private void UploadFile(Stream stream, string remoteFilePath)
    {
        using var client = CreateSFtpClient();
        client.Connect();
        client.UploadFile(stream, remoteFilePath.Replace("\\", "/"));
    }

    private void DownloadFile(Stream stream, string remoteFilePath)
    {
        using var client = CreateSFtpClient();
        client.Connect();
        var path = remoteFilePath.Replace("\\", "/");
        client.DownloadFile(path, stream);
    }

    private void DeleteFile(string remoteFilePath)
    {
        using var client = CreateSFtpClient();
        client.Connect();
        client.DeleteFile(remoteFilePath.Replace("\\", "/"));
    }

    private bool FileExists(string remoteFilePath)
    {
        using var client = CreateSFtpClient();
        client.Connect();
        return client.Exists(remoteFilePath.Replace("\\", "/"));
    }

    private bool DirectoryExists(string remoteFilePath)
    {
        using var client = CreateSFtpClient();
        client.Connect();
        return client.Exists(remoteFilePath.Replace("\\", "/"));
    }

    public SftpFileAttributes GetInfo(string remoteFilePath)
    {
        using var client = CreateSFtpClient();
        client.Connect();
        return client.GetAttributes(remoteFilePath.Replace("\\", "/"));
    }

    private void CreateDirectory(string remoteFilePath)
    {
        using var client = CreateSFtpClient();
        client.Connect();
        var path = remoteFilePath.Replace("\\", "/");

        var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        string currentPath = "";
        foreach (var part in parts)
        {
            currentPath += "/" + part;
            if (!client.Exists(currentPath))
            {
                client.CreateDirectory(currentPath);
            }
        }
    }

    private IEnumerable<string> ListFiles(string remoteDirectory)
    {
        using var client = CreateSFtpClient();
        client.Connect();
        return client.ListDirectory(remoteDirectory).Select(p => p.FullName);
    }


    public bool AttachmentsExists(string key)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return FileExists(filePath);
    }

    public bool ExportsExists(string key)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return FileExists(filePath);
    }

    public Stream GetFromAttachments(string id)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var stream = new MemoryStream();
        DownloadFile(stream, filePath);
        return stream;
    }

    public Stream GetFromExports(string id)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var stream = new MemoryStream();
        DownloadFile(stream, filePath);
        return stream;
    }

    public Stream GetFromImages(string id)
    {
        var location = Path.Combine(BaseDirectory, "images");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var stream = new MemoryStream();
        DownloadFile(stream, filePath);
        return stream;
    }

    public Stream GetFromImagesThumbnail(string key)
    {
        throw new NotImplementedException();
    }

    public Stream GetFromImagesThumnail(string id)
    {
        var location = Path.Combine(BaseDirectory, "images", "thumbnail");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var stream = new MemoryStream();
        DownloadFile(stream, filePath);
        return stream;
    }

    public Stream GetFromImports(string id)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(id).ToLower() + ".bin");
        var stream = new MemoryStream();
        DownloadFile(stream, filePath);
        return stream;
    }

    public bool ImagesExists(string key)
    {
        var location = Path.Combine(BaseDirectory, "images");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return FileExists(filePath);
    }

    public bool ImportsExists(string key)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return FileExists(filePath);
    }

    public void RemoveFromAttachments(string key)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (FileExists(filePath))
            DeleteFile(filePath);
    }

    public void RemoveFromExports(string key)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (FileExists(filePath))
            DeleteFile(filePath);
    }

    public void RemoveFromImages(string key)
    {
        var location = Path.Combine(BaseDirectory, "images");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (FileExists(filePath))
            DeleteFile(filePath);
    }

    public void RemoveFromImports(string key)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (FileExists(filePath))
            DeleteFile(filePath);
    }

    public void SaveToAttachments(string key, IFormFile file)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(file.OpenReadStream(), filePath);
    }

    public void SaveToAttachments(string key, Stream stream)
    {
        var location = Path.Combine(BaseDirectory, "attachments");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(stream, filePath);
    }

    public void SaveToExports(string key, IFormFile file)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(file.OpenReadStream(), filePath);
    }

    public void SaveToExports(string key, Stream stream)
    {
        var location = Path.Combine(BaseDirectory, "exports");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(stream, filePath);
    }

    public void SaveToImages(string key, IFormFile file)
    {
        var location = Path.Combine(BaseDirectory, "images");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(file.OpenReadStream(), filePath);
    }

    public void SaveToImages(string key, Stream stream)
    {
        var location = Path.Combine(BaseDirectory, "images");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(stream, filePath);
    }

    public void SaveToImports(string key, IFormFile file)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(file.OpenReadStream(), filePath);
    }

    public void SaveToImports(string key, Stream stream)
    {
        var location = Path.Combine(BaseDirectory, "imports");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(stream, filePath);
    }

    public void SaveTo(string path, IFormFile file)
    {
        throw new NotImplementedException();
    }

    public void SaveTo(string path, Stream stream)
    {
        throw new NotImplementedException();
    }

    public Stream GetFrom(string path)
    {
        var ms = new MemoryStream();
        DownloadFile(ms, path);
        return ms;
    }

    public void RemoveFrom(string path)
    {
        throw new NotImplementedException();
    }

    public void Save(string documentType, string key, IFormFile file, DateTime date)
    {
        documentType = documentType.ToLower();
        var location = Path.Combine(BaseDirectory, $"{documentType}/{date.Year}/{date.Month.ToString().PadLeft(2, '0')}");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(file.OpenReadStream(), filePath);
    }

    public void Save(string documentType, string key, Stream stream, DateTime date)
    {
        documentType = documentType.ToLower();
        var location = Path.Combine(BaseDirectory, $"{documentType}/{date.Year}/{date.Month.ToString().PadLeft(2, '0')}");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        UploadFile(stream, filePath);
    }

    public Stream Get(string documentType, string key, DateTime date)
    {
        documentType = documentType.ToLower();
        var location = Path.Combine(BaseDirectory, $"{documentType}/{date.Year}/{date.Month.ToString().PadLeft(2, '0')}");
        if (!DirectoryExists(location))
            CreateDirectory(location);

        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        var stream = new MemoryStream();
        DownloadFile(stream, filePath);
        return stream;
    }

    public void Remove(string documentType, string key, DateTime date)
    {
        documentType = documentType.ToLower();
        var location = Path.Combine(BaseDirectory, $"{documentType}/{date.Year}/{date.Month.ToString().PadLeft(2, '0')}");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        if (FileExists(filePath))
            DeleteFile(filePath);
    }

    public bool Exists(string documentType, string key, DateTime date)
    {
        documentType = documentType.ToLower();
        var location = Path.Combine(BaseDirectory, $"{documentType}/{date.Year}/{date.Month.ToString().PadLeft(2, '0')}");
        var filePath = Path.Combine(location, Security.SHA256Hash(key).ToLower() + ".bin");
        return FileExists(filePath);
    }
}

public class AwsS3Response
{
    public string Message { get; set; }
    public HttpStatusCode Status { get; set; }
    public bool IsHttpStatusCodeSuccess => Status == HttpStatusCode.OK;
}

public class GoogleConfig
{
    public string CredentialFile { get; set; }
    public string BucketName { get; set; }
    public string ProjectId { get; set; }
}