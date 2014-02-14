using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web;

namespace JCore.SitecoreModules.ImageCropping.Resources.Media
{
    public static class CustomMediaManager
    {
        /// <summary>
        /// The _provider
        /// </summary>
        private static CustomMediaProvider _provider;

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        public static CustomMediaProvider Provider
        {
            get
            {
                return CustomMediaManager._provider;
            }
            set
            {
                CustomMediaManager._provider = value;
            }
        }

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
                return CustomMediaManager.Provider.Effects;
            }
            set
            {
                CustomMediaManager.Provider.Effects = value;
            }
        }

        /// <summary>
        /// Initializes the <see cref="CustomMediaManager"/> class.
        /// </summary>
        static CustomMediaManager()
        {
            CustomMediaManager._provider = new CustomMediaProvider();
        }

        /// <summary>
        /// Gets the media URL.
        /// </summary>
        /// <param name="mediaItem">The media item.</param>
        /// <param name="options">The thumbnail options.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static string GetMediaUrl(global::Sitecore.Data.Items.MediaItem mediaItem, CustomMediaUrlOptions options)
        {
            return CustomMediaManager.Provider.GetMediaUrl(mediaItem, options);
        }

        /// <summary>
        /// Gets the media URL.
        /// </summary>
        /// <param name="mediaItem">The media item.</param>
        /// <returns></returns>
        public static string GetMediaUrl(ImageField imageField)
        {
            if (imageField == null)
                return string.Empty;
            var mediaItem = imageField.MediaItem;
            if (mediaItem == null)
                return string.Empty;

            CustomMediaUrlOptions originalUrlOptions = CustomMediaUrlOptions.GetMediaOptions(mediaItem);
            var options = CustomMediaUrlOptions.GetShellOptions();

            var height = 0;
            if (int.TryParse(imageField.Height, out height))
            {
                options.Height = height;
            }

            var width = 0;
            if (int.TryParse(imageField.Width, out width))
            {
                options.Width = width;
            }

            options.Language = mediaItem.Language;
            options.UseDefaultIcon = true;

            return CustomMediaManager.GetMediaUrl(imageField, options);
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
            
            string cropRegion = WebUtil.HtmlEncode(new XmlValue(imageField.Value, "image").GetAttribute("cropregion"));
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

            return CustomMediaManager.GetMediaUrl(mediaItem, options);
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
                return cropRegion.Split(',').Select(c => int.Parse(c)).ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex, typeof(CustomMediaManager));
            }
            return new int[4];
        }
    
    }
}
