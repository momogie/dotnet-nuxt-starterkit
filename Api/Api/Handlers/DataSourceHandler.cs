using Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared;
using System.ComponentModel.DataAnnotations;

namespace Api.Handlers;

[Authorize]
[Post("/api/data-source")]
public class DataSourceHandler([FromServices]AppDbContext appDb, [FromBody] DataSourceRequest request) : CommandHandler
{
    protected Type Type { get; set; }
    protected string Schema { get; set; }

    public override JsonSerializerSettings JsonSerializerSettings
    {
        get
        {
            if (request.Fields == null || request.Fields.Length == 0)
                return default;

            var resolver = new JsonIgnoreResolver();

            var props = Type.GetProperties().Select(p => p.Name).Where(p => !request.Fields.Contains(p)).ToList();

            foreach (var r in props)
                resolver.IgnoreProperty(Type, r); // hanya Code di Item yg di-ignore

            //var settings = new JsonSerializerSettings
            //{
            //    ContractResolver = resolver,
            //    Formatting = Formatting.Indented
            //};

            return new JsonSerializerSettings
            {
                ContractResolver = resolver,
            };
        }
    }

    public override async Task<IResult> Validate()
    {
        var exists = AppDbContext.ViewList.TryGetValue(string.Concat(request.DataSource, "View"), out (string schema, Type type) value);

        if (!exists)
            return NotFound();

        Type = value.type;
        Schema = value.schema;

        return await Next();
    }

    public override DataResult<object> Response()
    {
        var result = appDb.Views.Filter(Schema, Type, request);
        return result;
    }
}

public class DataSourceRequest : RequestParameter
{
    [Required]
    public string DataSource { get; set; }
}