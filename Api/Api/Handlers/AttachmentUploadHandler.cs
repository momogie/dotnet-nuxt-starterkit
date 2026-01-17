using Api.Entities;
using Api.Handlers.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Api.Handlers;

//[Authorize]
[Post("/api/attachment/upload")]
public class AttachmentUploadHandler([FromServices] AppDbContext appDb
    ,[FromForm] AttachmentUploadCommand command
    , IFileStorage fileStorage) : CommandHandler
{
    protected Attachment Data { get; set; }
    public override Task<IResult> Validate()
    {
        if (command?.File == null)
            AddError("File", "The File is required");

        return base.Validate();
    }

    [Pipeline(1)]
    public void Save()
    {
        if (command?.File == null) return;
        if(command.File.Length == 0) return;

        Data = new Attachment
        {
            Id = Guid.NewGuid().ToString(),
            Key = Guid.NewGuid().UniqueId(),
            DocumentId = command.DocumentId,
            DocumentType = command.DocumentType,
            FileName = command.File.FileName,
            FileSize = command.File.Length,
            FileType = command.File.ContentType,
        };
        fileStorage.SaveToAttachments(Data.Id, command.File);

        appDb.Attachments.Add(Data);
        appDb.SaveChanges();
    }

    public override string Response() => Data.Key;
}
