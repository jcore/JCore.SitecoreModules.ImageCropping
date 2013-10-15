using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.Resources;
using Sitecore.Resources.Media;
using Sitecore.Web;
using Sitecore;

namespace JCore.SitecoreModules.ImageCropping.Resources.Media
{
    public class CustomMediaProvider : MediaProvider
    {
        public CustomMediaProvider() : base() { }
        /// <summary>
        /// The effects
        /// </summary>
        private CustomImageEffects effects = new CustomImageEffects();

        /// <summary>
        /// Gets or sets the object handling image effects and transformations.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The effects.
        /// </value>
        public new CustomImageEffects Effects
        {
            get
            {
                return this.effects;
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.effects = value;
            }
        }
        /// <summary>
        /// Gets a media URL.
        /// </summary>
        /// <param name="item">The media item.</param>
        /// <param name="options">The query string.</param>
        /// <returns>
        /// The media URL.
        /// </returns>
        public override string GetMediaUrl(MediaItem item, MediaUrlOptions options)
        {
            Assert.ArgumentNotNull((object)item, "item");
            Assert.ArgumentNotNull((object)options, "options");
            bool flag = options.Thumbnail || this.HasMediaContent((Sitecore.Data.Items.Item)item);
            if (!flag && item.InnerItem["path"].Length > 0)
                return item.InnerItem["path"];
            if (options.UseDefaultIcon && !flag)
                return Themes.MapTheme(Settings.DefaultIcon);
            Assert.IsTrue(this.Config.MediaPrefixes[0].Length > 0, "media prefixes are not configured properly.");
            string str1 = this.MediaLinkPrefix;
            if (options.AbsolutePath)
                str1 = options.VirtualFolder + str1;
            else if (str1.StartsWith("/", StringComparison.InvariantCulture))
                str1 = StringUtil.Mid(str1, 1);
            if (options.AlwaysIncludeServerUrl)
                str1 = FileUtil.MakePath(WebUtil.GetServerUrl(), str1, '/');
            string str2 = StringUtil.EnsurePrefix('.', StringUtil.GetString(options.RequestExtension, item.Extension, "ashx"));
            string str3 = options.ToString();
            if (str3.Length > 0)
                str2 = str2 + "?" + str3;
            string str4 = "/sitecore/media library/";
            string path = item.InnerItem.Paths.Path;
            string str5 = !options.UseItemPath || !path.StartsWith(str4, StringComparison.OrdinalIgnoreCase) ? item.ID.ToShortID().ToString() : StringUtil.Mid(path, str4.Length);
            return str1 + str5 + (options.IncludeExtension ? str2 : string.Empty);
        }
    }
}
