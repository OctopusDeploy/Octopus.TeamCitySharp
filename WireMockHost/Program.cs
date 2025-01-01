// see source code for all the possible properties

using WireMock.Handlers;
using WireMock.Logging;
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

StandAloneApp.Start(settings);

Console.WriteLine("Press any key to stop the server");
Console.ReadKey();
