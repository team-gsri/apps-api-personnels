using System.Net.Http.Json;

namespace Gsri.Api.Personnels.Tests.Utils.Handlers;

internal class DeleteHandler : ITestHandler
{
    public Task<HttpResponseMessage> Handle(HttpClient client, string base_url, string key, JsonContent content) =>
        client.DeleteAsync($"{base_url}/{key}");
}
