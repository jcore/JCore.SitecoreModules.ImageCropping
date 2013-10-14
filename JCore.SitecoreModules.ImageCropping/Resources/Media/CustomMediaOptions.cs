using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Resources.Media;

namespace JCore.SitecoreModules.ImageCropping.Resources.Media
{
    public class CustomMediaOptions : MediaOptions
    {
        private string CropRegion;
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMediaOptions"/> class.
        /// </summary>
        /// <param name="mediaOptions">The media options.</param>
        public CustomMediaOptions(MediaOptions mediaOptions)
        {
            this.AllowStretch = mediaOptions.AllowStretch;
            this.BackgroundColor = mediaOptions.BackgroundColor;
            this.Height = mediaOptions.Height;
            this.IgnoreAspectRatio = mediaOptions.IgnoreAspectRatio;
            this.MaxHeight = mediaOptions.MaxHeight;
            this.MaxWidth = mediaOptions.MaxWidth;
            this.Scale = mediaOptions.Scale;
            this.Thumbnail = mediaOptions.Thumbnail;
            this.UseMediaCache = this.UseMediaCache;
            this.Width = mediaOptions.Width;
            if (mediaOptions.CustomOptions.ContainsKey("cropregion"))
            {
                this.CropRegion = mediaOptions.CustomOptions.FirstOrDefault(p => p.Key == "cropregion").Value;
            }           
        }
        /// <summary>
        /// Gets the image options.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The transformation options.
        /// 
        /// </returns>
        public new CustomTransformationOptions GetTransformationOptions()
        {
            return new CustomTransformationOptions()
            {
                AllowStretch = this.AllowStretch,
                BackgroundColor = this.BackgroundColor,
                IgnoreAspectRatio = this.IgnoreAspectRatio,
                MaxSize = new Size(this.MaxWidth, this.MaxHeight),
                Scale = this.Scale,
                Size = new Size(this.Width, this.Height),
                CropRegion = this.CropRegion.Split(',')
            };
        }
    }
}
