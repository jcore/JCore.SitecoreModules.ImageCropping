using System;
using System.Linq;
using System.Web;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web;

namespace JCore.SitecoreModules.ImageCropping.Resources.Media
{
    public static class CustomMediaManager
    {
        public static MediaCache Cache
        {
            get
            {
                return MediaManager.Provider.Cache;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                MediaManager.Provider.Cache = value;
            }
        }

        public static MediaConfig Config
        {
            get
            {
                return Provider.Config;
            }
            set
            {
                Provider.Config = value;
            }
        }
        /// <summary>
        /// Gets or sets the object used when creating new media items.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The creator.
        /// </value>
        public static MediaCreator Creator
        {
            get
            {
                return Provider.Creator;
            }
            set
            {
                Provider.Creator = value;
            }
        }


        public static CustomMediaProvider Provider { get; set; }

        /// <summary>
        /// Gets or sets the object handling image effects and transformations.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The effects.
        /// </value>
        public static CustomImageEffects Effects
        {
            get
            {
                return Provider.Effects;
            }
            set
            {
                Provider.Effects = value;
            }
        }

        /// <summary>
        /// Gets the media link prefix.
        ///             The prefix to use when Sitecore generates media links.
        ///             The setting is used in the front-end as well as the back-end.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The media link prefix.
        /// </value>
        public static string MediaLinkPrefix
        {
            get
            {
                return Provider.MediaLinkPrefix;
            }
        }
        /// <summary>
        /// Gets or sets the MIME resolver.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The MIME resolver.
        /// </value>
        public static MimeResolver MimeResolver
        {
            get
            {
                return Provider.MimeResolver;
            }
            set
            {
                Provider.MimeResolver = value;
            }
        }

        static CustomMediaManager()
        {
            var configNode = Factory.GetConfigNode("mediaLibrary/mediaProvider");
            if (configNode == null)
                Provider = new CustomMediaProvider();
            else
                Provider = Factory.CreateObject(configNode, true) as CustomMediaProvider;
        }
        /// <summary>
        /// Gets media from a media item.
        /// 
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns/>
        public static Sitecore.Resources.Media.Media GetMedia(MediaItem item)
        {
            return Provider.GetMedia(item);
        }
        /// <summary>
        /// Gets media from a media URI.
        /// 
        /// </summary>
        /// <param name="mediaUri">The media URI.</param>
        /// <returns/>
        public static Sitecore.Resources.Media.Media GetMedia(MediaUri mediaUri)
        {
            return Provider.GetMedia(mediaUri);
        }
        /// <summary>
        /// Determines whether the specified item has a media stream with content.
        /// 
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// <c>true</c> if  the specified item has a media stream with content; otherwise, <c>false</c>.
        /// 
        /// </returns>
        public static bool HasMediaContent(Item item)
        {
            return Provider.HasMediaContent(item);
        }
        /// <summary>
        /// Gets a media URL.
        /// 
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The media URL.
        /// </returns>
        public static string GetMediaUrl(MediaItem item)
        {
            return Provider.GetMediaUrl(item);
        }

        /// <summary>
        /// Gets a media URL.
        /// </summary>
        public static string GetMediaUrl(MediaItem item, CustomMediaUrlOptions options)
        {
            return Provider.GetMediaUrl(item, options);
        }

        public static string GetThumbnailUrl(MediaItem item)
        {
            return Provider.GetThumbnailUrl(item);
        }
        public static bool IsMediaRequest(HttpRequest httpRequest)
        {
            return Provider.IsMediaRequest(httpRequest);
        }
        public static bool IsMediaUrl(string url)
        {
            return Provider.IsMediaUrl(url);
        }
        public static MediaRequest ParseMediaRequest(HttpRequest request)
        {
            return Provider.ParseMediaRequest(request);
        }

        /// <summary>
        /// Gets the media URL.
        /// </summary>
        /// <returns></returns>
        public static string GetMediaUrl(ImageField imageField)
        {
            if (imageField == null)
                return string.Empty;
            var mediaItem = imageField.MediaItem;
            if (mediaItem == null)
                return string.Empty;

            //var options = CustomMediaUrlOptions.GetMediaUrlOptions(mediaItem);
            var options = CustomMediaUrlOptions.Empty;

            int height;
            if (int.TryParse(imageField.Height, out height))
            {
                options.Height = height;
            }

            int width;
            if (int.TryParse(imageField.Width, out width))
            {
                options.Width = width;
            }

            options.Language = mediaItem.Language;
            options.UseDefaultIcon = true;

            return GetMediaUrl(imageField, options);
        }

        /// <summary>
        /// Gets the media URL.
        /// </summary>
        /// <param name="imageField">The image field.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static string GetMediaUrl(ImageField imageField, CustomMediaUrlOptions options)
        {
            if (imageField == null)
                return string.Empty;
            var mediaItem = imageField.MediaItem;
            if (mediaItem == null)
                return string.Empty;

            string cropRegion = HttpUtility.HtmlEncode(new XmlValue(imageField.Value, "image").GetAttribute("cropregion"));
            if (!string.IsNullOrEmpty(cropRegion))
            {
                try
                {
                    options.CropRegion = cropRegion;
                    var coordinates = ConvertToIntArray(cropRegion);
                    if (options.Width + options.Height > (coordinates[2] - coordinates[0] + coordinates[3] + coordinates[1]))
                    {
                        options.Width = coordinates[2] - coordinates[0];
                        options.Height = coordinates[3] + coordinates[1];
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex, typeof(CustomMediaManager));
                }
            }
            if (!Settings.Media.RequestProtection.Enabled || imageField.InnerField.Name.StartsWith("__"))
                return GetMediaUrl(mediaItem, options); 
            return HashingUtils.ProtectAssetUrl(GetMediaUrl(mediaItem, options));
        }

        /// <summary>
        /// Converts to int array.
        /// </summary>
        /// <param name="cropRegion">The crop region.</param>
        /// <returns></returns>
        public static int[] ConvertToIntArray(string cropRegion)
        {
            try
            {
                return cropRegion.Split(',').Select(int.Parse).ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex, typeof(CustomMediaManager));
            }
            return new int[4];
        }

    }


}
