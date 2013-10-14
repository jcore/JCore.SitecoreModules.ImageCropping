using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Resources.Media;

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
        /// <param name="thumbnailOptions">The thumbnail options.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static string GetMediaUrl(global::Sitecore.Data.Items.MediaItem mediaItem, CustomMediaUrlOptions options)
        {
            return CustomMediaManager.Provider.GetMediaUrl(mediaItem, options);
        }
    }
}
