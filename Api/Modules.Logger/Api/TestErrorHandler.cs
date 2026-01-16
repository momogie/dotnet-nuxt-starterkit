using Microsoft.AspNetCore.Http;
using Shared;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Modules.Logger.Api;

//[Authorize]
[Get("/api/logger/test")]
public class TestErrorHandler : CommandHandler
{
    public override Task<IResult> Validate()
    {
        throw new NotImplementedException();
        return base.Validate();
    }
}
