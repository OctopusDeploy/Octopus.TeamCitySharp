// see source code for all the possible properties

using Newtonsoft.Json;
using WireMock.Admin.Mappings;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Net.StandAlone;
using WireMock.Settings;
using WireMock.Types;

var wiremockFolder = "/Users/matt/dev/Octopus/Octopus.TeamCitySharp/src/Tests/IntegrationTests/wiremock";
var settings = new WireMockServerSettings
{
    Urls = ["http://localhost:8112"],
    ProxyAndRecordSettings = new ProxyAndRecordSettings
    {
        Url = "http://localhost:8111",
        SaveMapping = true,
        SaveMappingToFile = true,
        ExcludedHeaders = ["Host", "traceparent"],
        PrefixForSavedMappingFile = "proxy_mapping",
    },
    WatchStaticMappings = true,
    AllowPartialMapping = false,
    QueryParameterMultipleValueSupport = QueryParameterMultipleValueSupport.Ampersand,
    ReadStaticMappings = true,
    StartAdminInterface = true,
    FileSystemHandler = new LocalFileSystemHandler(wiremockFolder),
    Logger = new WireMockConsoleLogger(),
};

// var patternModel = new CustomPathParamMatcherModel("/customer/{customerId}/document/{documentId}",
//     new Dictionary<string, string>(2)
//     {
//         { "customerId", @"^[0-9]+$" },
//         { "documentId", @"^[0-9a-zA-Z\-\_]+\.[a-zA-Z]+$" }
//     });
// var model = new MatcherModel
// {
//     Name = nameof(CustomPathParamMatcher),
//     Pattern = JsonConvert.SerializeObject(patternModel)
// };



StandAloneApp.Start(settings);

Console.WriteLine("Press any key to stop the server");
Console.ReadKey();
