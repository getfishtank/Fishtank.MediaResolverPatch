using System.Web;
using Sitecore.Resources.Media;
using Sitecore.Resources;
using Sitecore.Configuration;
using Sitecore;
using System.Xml;
using Sitecore.Common;

namespace Fishtank.MediaResolverPatch
{
    /// <summary>
    /// Intercept the media request handler and apply custom caching to certain media files
    /// based on their extension and configured in GlobalConfiguration.MediaExtensionsNotToClientCache
    /// **Modified 2014-06-16 - James Gusa
    /// This code base is taken from Sitecore.Kernel.DLL and altered so that media library requests aren't encoded. 
    /// 
    /// Sitecore Patch Note:
    /// When rendering media URLs, the system did not use the configuration in the encodeNameReplacements section to replace special characters in the URLs. 
    /// This has been fixed so that media URLs also use the encodeNameReplacements configuration. (323105, 314977)
    /// 
    /// </summary>

    using Assert = Sitecore.Diagnostics.Assert;

    public class FishtankMediaConfig : MediaConfig
    {
        public FishtankMediaConfig() : base()
        {
        }
        public FishtankMediaConfig(XmlNode configNode) : base(configNode)
        {
        }
        public override MediaRequest ConstructMediaRequestInstance(HttpRequest httpRequest)
        {
            Assert.ArgumentNotNull((object)httpRequest, "httpRequest");
            FishtankMediaRequest mediaRequest = new FishtankMediaRequest();
            if (mediaRequest == null)
                return (MediaRequest)null;
            mediaRequest.Initialize(httpRequest);
            return (MediaRequest)mediaRequest;
        }

    }

}
