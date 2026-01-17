using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.Api.Import.Commands;
using Shared;

namespace Modules.Common.Api.Import.Handlers;

[Authorize]
[Post("/api/common/import")]
public class ImportHandler([FromForm] ImportCommand command) : CommandHandler
{
    protected Entities.Import Data { get; set; }
    public override Task<IResult> Validate()
    {
        return base.Validate();
    }

    public void Save()
    {
        Data = new Entities.Import
        {
            Id = Guid.NewGuid().ToString(),
        };
    }

    public void SaveAttachment()
    {

    }

    public override object Response()
    {
        return base.Response();
    }
}
