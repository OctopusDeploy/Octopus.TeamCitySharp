using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WireMock.Admin.Mappings;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;

namespace TeamCitySharp.IntegrationTests
{
    public static class Configuration
    {
        private static readonly WireMockServer WiremockServer;
        private static readonly IConfigurationRoot configuration;

        static Configuration()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var wiremockFolder = Path.GetFullPath(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName!, "..", "..", "..", "wiremock"));
            var settings = new WireMockServerSettings
            {
                Urls = ["http://localhost:8112"],
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "http://localhost:8111",
                    
                    //enable these two if you are adding or updating tests 
                    //SaveMapping = true,
                    //SaveMappingToFile = true,
                    
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
            WiremockServer = WireMockServer.Start(settings);
        }

        public static string GetAppSetting(string key) => configuration[key];

        public static HttpClient GetWireMockClient() => WiremockServer.CreateClient();
    }
}
