using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gsri.Api.Personnels.Utils.Swagger;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services) => services
        .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
        .AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>())
        .AddApiVersioning().AddMvc().AddApiExplorer(options =>
        {
            options.SubstituteApiVersionInUrl = true;
            options.GroupNameFormat = "'v'VVV";
        }).Services;
}
