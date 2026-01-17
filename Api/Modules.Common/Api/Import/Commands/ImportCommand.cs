using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Common.Api.Import.Commands;

public class ImportCommand
{
    public IFormFile File { get; set;  }
    public string Description { get; set; }
}
