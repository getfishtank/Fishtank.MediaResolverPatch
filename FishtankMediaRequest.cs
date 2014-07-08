using System.Web;
using Sitecore.Resources.Media;
using System;
using Sitecore.Data.Items;
using Sitecore;
using Sitecore.Data;
using System.Globalization;


namespace Fishtank.MediaResolverPatch
{
    /// <summary>
    /// Intercept the media request handler and apply custom caching to certain media files
    /// based on their extension and configured in GlobalConfiguration.MediaExtensionsNotToClientCache
    /// 
    /// **Modified 2014-06-16
    /// This code base is taken from Sitecore.Kernel.DLL and altered so that media library requests aren't encoded. 
    /// Sitecore Patch Note:
    /// When rendering media URLs, the system did not use the configuration in the encodeNameReplacements section to replace special characters in the URLs. 
    /// This has been fixed so that media URLs also use the encodeNameReplacements configuration. (323105, 314977)
    /// </summary>



    public class FishtankMediaRequest : MediaRequest
    {
        protected override string GetMediaPath(string localPath)
        {
            int indexA = -1;
            string strB = string.Empty;
            foreach (string str in MediaManager.Provider.Config.MediaPrefixes)
            {
                indexA = localPath.IndexOf(str, StringComparison.InvariantCultureIgnoreCase);
                if (indexA >= 0)
                {
                    strB = str;
                    break;
                }
            }
            if (indexA < 0 || string.Compare(localPath, indexA, strB, 0, strB.Length, true, CultureInfo.InvariantCulture) != 0)
                return string.Empty;
            string id = StringUtil.Divide(StringUtil.Mid(localPath, indexA + strB.Length), '.', true)[0];
            if (id.EndsWith("/", StringComparison.InvariantCulture))
                return string.Empty;
            if (ShortID.IsShortID(id))
                return ShortID.Decode(id);
            //2014-06-16 JG - Removed decode call below
            return "/sitecore/media library/" + id.TrimStart(new char[1] { '/' });;
        }
    }

}
