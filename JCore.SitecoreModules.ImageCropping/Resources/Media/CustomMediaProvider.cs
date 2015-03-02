using System;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.IO;
using Sitecore.Resources;
using Sitecore.Resources.Media;
using Sitecore.Web;

namespace JCore.SitecoreModules.ImageCropping.Resources.Media
{
    public class CustomMediaProvider : MediaProvider
    {
        /// <summary>
        /// The effects
        /// </summary>
        private CustomImageEffects _effects = new CustomImageEffects();

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
                return _effects;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                _effects = value;
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
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(options, "options");
            var flag = options.Thumbnail || HasMediaContent(item);
            if (!flag && item.InnerItem["path"].Length > 0)
            {
                if (!options.LowercaseUrls)
                    return item.InnerItem["path"];
                return item.InnerItem["path"].ToLowerInvariant();
            }
            if (options.UseDefaultIcon && !flag)
            {
                return !options.LowercaseUrls ? Themes.MapTheme(Settings.DefaultIcon) : Themes.MapTheme(Settings.DefaultIcon).ToLowerInvariant();
            }
            Assert.IsTrue(Config.MediaPrefixes[0].Length > 0, "media prefixes are not configured properly.");
            var str1 = MediaLinkPrefix;
            if (options.AbsolutePath)
                str1 = options.VirtualFolder + str1;
            else if (str1.StartsWith("/", StringComparison.InvariantCulture))
                str1 = StringUtil.Mid(str1, 1);
            var part2 = MainUtil.EncodePath(str1, '/');
            if (options.AlwaysIncludeServerUrl)
                part2 = FileUtil.MakePath(string.IsNullOrEmpty(options.MediaLinkServerUrl) ? WebUtil.GetServerUrl() : options.MediaLinkServerUrl, part2, '/');
            var str2 = StringUtil.EnsurePrefix('.', StringUtil.GetString(options.RequestExtension, item.Extension, "ashx"));
            var str3 = options.ToString();
            if (str3.Length > 0)
                str2 = str2 + "?" + str3;
            const string str4 = "/sitecore/media library/";
            var path = item.InnerItem.Paths.Path;
            var str5 = MainUtil.EncodePath(!options.UseItemPath || !path.StartsWith(str4, StringComparison.OrdinalIgnoreCase) ? item.ID.ToShortID().ToString() : StringUtil.Mid(path, str4.Length), '/');
            var str6 = part2 + str5 + (options.IncludeExtension ? str2 : string.Empty);
            return !options.LowercaseUrls ? str6 : str6.ToLowerInvariant();
        }
    }
}
