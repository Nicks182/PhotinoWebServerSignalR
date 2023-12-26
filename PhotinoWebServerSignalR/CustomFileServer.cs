
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.Net.NetworkInformation;

using Microsoft.Extensions.DependencyInjection;

namespace PhotinoWebServerSignalR
{
    public class CustomFileServer
    {
        public static WebApplication CreateStaticFileServer(CustomFileServerOptions options, out string baseUrl)
        {
            var builder = WebApplication
            .CreateBuilder(new WebApplicationOptions()
            {
                Args = options.Args,
            });

            builder.Services.AddSignalR().AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(null, true));
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
                options.PayloadSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            });

            int port = options.PortStart;

            // Try ports until available port is found
            while (IPGlobalProperties
                .GetIPGlobalProperties()
                .GetActiveTcpListeners()
                .Any(x => x.Port == port))
            {
                if (port > options.PortStart + options.PortRange)
                {
                    throw new SystemException($"Couldn't find open port within range {options.PortStart} - {options.PortStart + options.PortRange}.");
                }

                port++;
            }

            baseUrl = $"http://localhost:{port}";

            if (options.AllowLocalAccess)
            {
                builder.WebHost.UseUrls($"http://*:{port}");
            }
            else
            {
                builder.WebHost.UseUrls(baseUrl);
            }

            WebApplication app = builder.Build();


            EmbeddedFileProvider fp = _Get_FileProvider();

            app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = fp });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fp,
            });

            app.MapHub<SignalRHub>("MessageHub");

            return app;
        }

        private static EmbeddedFileProvider _Get_FileProvider()
        {
            return new EmbeddedFileProvider(
                assembly: typeof(CustomFileServer).Assembly,
                baseNamespace: "PhotinoWebServerSignalR.wwwroot");
        }

    }

    public class CustomFileServerOptions
    {
        public string[] Args { get; set; }
        public int PortStart { get; set; }
        public int PortRange { get; set; }
        public string WebRootFolder { get; set; }

        public bool AllowLocalAccess { get; set; }
    }
}
