using AspNetCore.Scalar;
using Microsoft.OpenApi;
//using Microsoft.OpenApi.Models;

namespace Api;

public static class OpenApiExtension
{
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            //options.SwaggerDoc("althea", new OpenApiInfo
            //{
            //    Title = "Althea API",
            //    Version = "v1",
            //    Description = "API untuk modul Althea"
            //});

            //options.SwaggerDoc("external", new OpenApiInfo
            //{
            //    Title = "External API",
            //    Version = "v1",
            //    Description = "API untuk sub apps"
            //});

            //options.DocInclusionPredicate((docName, apiDesc) =>
            //{
            //    if (docName == "external")
            //        return apiDesc.RelativePath.StartsWith("api/external");

            //    if (docName == "althea")
            //        return true;

            //    return false;
            //});

            //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //{
            //    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
            //    Name = "Authorization",
            //    In = ParameterLocation.Header,
            //    Type = SecuritySchemeType.ApiKey,
            //    Scheme = "Bearer"
            //});

            //options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
            //{
            //    Description = "Basic Authorization header using the Basic scheme (Example: 'Basic 12345abcdef')",
            //    Name = "Authorization",
            //    In = ParameterLocation.Header,
            //    Type = SecuritySchemeType.ApiKey,
            //    Scheme = "Basic"
            //});

        });
        return services;
    }

    public static void UseApiDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseScalar(options =>
        {
            //options.UseSpecUrl("/swagger/v1/swagger.json");
            ////options.RoutePrefix = "scalar-api-docs/althea";
            //options.UseTheme(Theme.Default);
        });
        //app.UseScalar(options =>
        //{
        //    options.UseSpecUrl("/swagger/external/swagger.json");
        //    options.RoutePrefix = "scalar-api-docs/external";
        //    options.UseTheme(Theme.Default);
        //});
    }
}
