using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Configuration;
[assembly: OwinStartup(typeof(WiicoApi.Startup))]

namespace WiicoApi
{
    public class Startup
    {
        private string mqttConnectUrl = ConfigurationManager.AppSettings["ConnectMQTTUrl"].ToString();
        private string accessKey = ConfigurationManager.AppSettings["MQTTAccessKey"].ToString();
        private string iotUrl = ConfigurationManager.AppSettings["IOTUrl"].ToString();
        public void Configuration(IAppBuilder app)
        {
            //SignalR retains 20 messages in memory per hub per connection
            GlobalHost.Configuration.DefaultMessageBufferSize = 20;
            //GlobalHost.DependencyResolver.UseRedis("127.0.0.1", 6379, "", "iThinkHub");
            // Branch the pipeline here for requests that start with "/signalr"
            app.Map("/signalr", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {

                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    EnableJSONP = true
                };
                hubConfiguration.EnableDetailedErrors = true;
                app.MapSignalR(hubConfiguration);
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
        }

    }
}
