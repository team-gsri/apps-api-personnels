using System.Net.Http.Json;

namespace Gsri.Api.Personnels.Tests.Utils.Handlers;

internal class PutHandler : ITestHandler
{
    public Task<HttpResponseMessage> Handle(HttpClient client, string base_url, string key, JsonContent content) =>
        client.PutAsync($"{base_url}/{key}", content);
}
