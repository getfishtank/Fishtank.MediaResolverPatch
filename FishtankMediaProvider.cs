using Sitecore.Resources.Media;
using System;
using Sitecore.Data.Items;
using Sitecore.Resources;
using Sitecore.Configuration;
using Sitecore;
using Sitecore.IO;
using Sitecore.Web;
using Sitecore.Data;
using System.Globalization;
using System.Xml;


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
    

    using Assert = Sitecore.Diagnostics.Assert;
   
    public class FishtankMediaProvider : MediaProvider
    {
        private FishtankMediaConfig config;

        public override MediaConfig Config
        {
            get
            {
                return (MediaConfig)this.config;
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.config = (FishtankMediaConfig)value;
            }
        }
        public FishtankMediaProvider()
        {
            XmlNode configNode = Factory.GetConfigNode("mediaLibrary");
            if (configNode != null)
                this.config = new FishtankMediaConfig(configNode);
            else
                this.config = new FishtankMediaConfig();
        }

        public override string GetMediaUrl(MediaItem item, MediaUrlOptions options)
        {
            Assert.ArgumentNotNull((object)item, "item");
            Assert.ArgumentNotNull((object)options, "options");
            bool flag = options.Thumbnail || this.HasMediaContent((Sitecore.Data.Items.Item)item);
            if (!flag && item.InnerItem["path"].Length > 0)
            {
                if (!options.LowercaseUrls)
                    return item.InnerItem["path"];
                else
                    return item.InnerItem["path"].ToLowerInvariant();
            }
            else if (options.UseDefaultIcon && !flag)
            {
                if (!options.LowercaseUrls)
                    return Themes.MapTheme(Settings.DefaultIcon);
                else
                    return Themes.MapTheme(Settings.DefaultIcon).ToLowerInvariant();
            }
            else
            {
                Assert.IsTrue(this.Config.MediaPrefixes[0].Length > 0, "media prefixes are not configured properly.");
                string str1 = this.MediaLinkPrefix;
                if (options.AbsolutePath)
                    str1 = options.VirtualFolder + str1;
                else if (str1.StartsWith("/", StringComparison.InvariantCulture))
                    str1 = StringUtil.Mid(str1, 1);
                string part2 = MainUtil.EncodePath(str1, '/');
                if (options.AlwaysIncludeServerUrl)
                    part2 = FileUtil.MakePath(string.IsNullOrEmpty(options.MediaLinkServerUrl) ? WebUtil.GetServerUrl() : options.MediaLinkServerUrl, part2, '/');
                string str2 = StringUtil.EnsurePrefix('.', StringUtil.GetString(options.RequestExtension, item.Extension, "ashx"));
                string str3 = options.ToString();
                if (str3.Length > 0)
                    str2 = str2 + "?" + str3;
                string str4 = "/sitecore/media library/";
                string path = item.InnerItem.Paths.Path;
                //2014-06-16 JG - Removed decode call below
                string str5 = !options.UseItemPath || !path.StartsWith(str4, StringComparison.OrdinalIgnoreCase) ? item.ID.ToShortID().ToString() : StringUtil.Mid(path, str4.Length);
                string str6 = part2 + str5 + (options.IncludeExtension ? str2 : string.Empty);
                if (!options.LowercaseUrls)
                    return str6;
                else
                    return str6.ToLowerInvariant();
            }
        }

    }
  
}
