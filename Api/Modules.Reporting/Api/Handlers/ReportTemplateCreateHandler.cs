using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Modules.Reporting.Entities;
using Shared;

namespace Modules.Reporting.Api.Handlers;

[Authorize]
[Post("/api/reporting/template/create")]
public class ReportTemplateCreateHandler(AppDbContext appDb, IFileStorage fileStorage) : CommandHandler
{
    protected Attachment Data { get; set; }
    public override Task<IResult> Validate()
    {
        return base.Validate();
    }

    [Pipeline(1)]
    public void Save()
    {
        Data = new Attachment
        {
            Id = Guid.NewGuid().ToString(),
            Key = Guid.NewGuid().UniqueId(),
        };

        appDb.Attachments.Add(Data);
        appDb.SaveChanges();
    }

    public override string Response() => Data.Key;
}
