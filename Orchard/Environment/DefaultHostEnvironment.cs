using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Services;
using Orchard.Utility.Extensions;

namespace Orchard.Environment
{
    public class DefaultHostEnvironment : HostEnvironment
    {
        private const string WebConfigPath = "~/web.config";
        private const string RefreshHtmlPath = "~/refresh.html";
        private const string HostRestartPath = "~/bin/HostRestart";
        private readonly IClock _clock;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public DefaultHostEnvironment(IClock clock, IHttpContextAccessor httpContextAccessor)
        {
            _clock = clock;
            _httpContextAccessor = httpContextAccessor;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public override void RestartAppDomain()
        {
            if (IsFullTrust)
            {
                HttpRuntime.UnloadAppDomain();
            }
            else
            {
                bool success = TryWriteBinFolder() || TryWriteWebConfig();

                if (!success)
                {
                    throw new CoreException(
                        "Orchard needs to be restarted due to a configuration change, but was unable to do so.\r\n" +
                        "To prevent this issue in the future, a change to the web server configuration is required:\r\n" +
                        "- run the application in a full trust environment, or\r\n" +
                        "- give the application write access to the '{0}' folder, or\r\n" +
                        "- give the application write access to the '{1}' file.",
                        HostRestartPath, WebConfigPath);
                }
            }

            // If setting up extensions/modules requires an AppDomain restart, it's very unlikely the
            // current request can be processed correctly.  So, we redirect to the same URL, so that the
            // new request will come to the newly started AppDomain.
            var httpContext = _httpContextAccessor.Current();
            if (httpContext != null)
            {
                // Don't redirect posts...
                if (httpContext.Request.RequestType == "GET")
                {
                    httpContext.Response.Redirect(HttpContext.Current.Request.ToUrlString(), true /*endResponse*/);
                }
                else
                {
                    httpContext.Response.ContentType = "text/html";
                    httpContext.Response.WriteFile(RefreshHtmlPath);
                    httpContext.Response.End();
                }
            }

        }

        private bool TryWriteWebConfig()
        {
            try
            {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(MapPath(WebConfigPath), DateTime.Now);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryWriteBinFolder()
        {
            try
            {
                var binMarker = MapPath(HostRestartPath);
                Directory.CreateDirectory(binMarker);

                using (var stream = File.CreateText(Path.Combine(binMarker, "marker.txt")))
                {
                    stream.WriteLine("Restart on '{0}'", DateTime.Now);
                    stream.Flush();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
