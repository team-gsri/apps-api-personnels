using Gsri.Api.Personnels.Database;
using Gsri.Api.Personnels.Joueurs.Implements;
using Gsri.Api.Personnels.Utils.Swagger;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors();
}

builder.Services.AddSwaggerServices();
builder.Services.AddControllers();
builder.Services.AddScoped<JoueursAdapter>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddApiVersioning().AddMvc().AddApiExplorer();
builder.Services.AddDbContext<PersonnelsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(PersonnelsDbContext)));
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseCors();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var group in app.DescribeApiVersions().Select(description => description.GroupName))
    {
        options.SwaggerEndpoint($"/swagger/{group}/swagger.yaml", group);
    }
});

app.Map(string.Empty, () => TypedResults.Redirect("/swagger"));
app.MapControllers();

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<PersonnelsDbContext>();
    await database.Database.MigrateAsync().ConfigureAwait(false);
}

await app.RunAsync().ConfigureAwait(false);
