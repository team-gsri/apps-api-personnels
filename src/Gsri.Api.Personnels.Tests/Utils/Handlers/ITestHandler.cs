using System.Net.Http.Json;

namespace Gsri.Api.Personnels.Tests.Utils.Handlers;

internal interface ITestHandler
{
    Task<HttpResponseMessage> Handle(HttpClient client, string base_url, string key, JsonContent content);
}
