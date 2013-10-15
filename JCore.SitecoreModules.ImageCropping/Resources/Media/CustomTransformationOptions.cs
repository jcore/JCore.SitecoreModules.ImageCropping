using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Configuration;

namespace JCore.SitecoreModules.ImageCropping.Resources.Media
{
    /// <summary>
    /// CustomTransformationOptions class
    /// 
    /// </summary>
    public class CustomTransformationOptions
    {
        /// <summary>
        /// The background color.
        /// 
        /// </summary>
        private Color backgroundColor = Color.Empty;
        /// <summary>
        /// The preserve resolution.
        /// 
        /// </summary>
        private bool preserveResolution = true;
        /// <summary>
        /// The maximum size;
        /// 
        /// </summary>
        private Size maxSize;
        /// <summary>
        /// The scale.
        /// 
        /// </summary>
        private float scale;
        /// <summary>
        /// The size.
        /// 
        /// </summary>
        private Size size;

        /// <summary>
        /// Gets or sets a value indicating whether stretching is allowed.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// <c>true</c> if stretching is allowed; otherwise, <c>false</c>.
        /// 
        /// </value>
        public bool AllowStretch { get; set; }

        /// <summary>
        /// Gets or sets the background color to use when scaling images.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The color of the background.
        /// </value>
        public Color BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                this.backgroundColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the compositing mode.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The compositing mode.
        /// 
        /// </value>
        public CompositingMode CompositingMode { get; set; }

        /// <summary>
        /// Gets or sets the interpolation mode.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The interpolation mode.
        /// 
        /// </value>
        public InterpolationMode InterpolationMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the aspect ratio when resizing.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// <c>true</c> if ignore aspect ratio; otherwise, <c>false</c>.
        /// 
        /// </value>
        public bool IgnoreAspectRatio { get; set; }

        /// <summary>
        /// Gets or sets the max size.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The max size.
        /// </value>
        public Size MaxSize
        {
            get
            {
                return this.maxSize;
            }
            set
            {
                this.maxSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the pixel offset mode.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The pixel offset mode.
        /// 
        /// </value>
        public PixelOffsetMode PixelOffsetMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="T:Sitecore.Resources.Media.TransformationOptions"/> preserves the resolution from original image.
        ///             When generating thumbnails, default resolution is used (96dpi), not the resolution from original image.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// <c>true</c> if the <see cref="T:Sitecore.Resources.Media.TransformationOptions"/> preserves the resolution; otherwise, <c>false</c>. Default is <c>true</c>.
        /// 
        /// </value>
        public bool PreserveResolution
        {
            get
            {
                return this.preserveResolution;
            }
            set
            {
                this.preserveResolution = value;
            }
        }

        /// <summary>
        /// Gets or sets the scale.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// Note that <c>Scale</c> takes precedence over <c>Size</c>, so after assigning a value to <c>Scale</c>, <c>Size</c> will be ignored.
        /// 
        /// </remarks>
        /// 
        /// <value>
        /// The scale.
        /// </value>
        public float Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                this.scale = value;
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The size.
        /// </value>
        public Size Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
            }
        }

        /// <summary>
        /// Gets or sets the quality of scaled image in percentage.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The quality.
        /// 
        /// </value>
        public int Quality { get; set; }

        /// <summary>
        /// The crop coordinates
        /// </summary>
        public string[] CropRegion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Resources.Media.TransformationOptions"/> class.
        /// 
        /// </summary>
        public CustomTransformationOptions()
        {
            this.Quality = Settings.Media.Resizing.Quality;
            this.CompositingMode = Settings.Media.Resizing.CompositingMode;
            this.InterpolationMode = Settings.Media.InterpolationMode;
            this.PixelOffsetMode = Settings.Media.Resizing.PixelOffsetMode;
        }

        /// <summary>
        /// Clones this instance.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The transformation options.
        /// 
        /// </returns>
        public CustomTransformationOptions Clone()
        {
            return new CustomTransformationOptions()
            {
                backgroundColor = this.backgroundColor,
                scale = this.scale,
                size = this.size,
                maxSize = this.maxSize,
                Quality = this.Quality,
                CompositingMode = this.CompositingMode,
                InterpolationMode = this.InterpolationMode,
                PixelOffsetMode = this.PixelOffsetMode,
                CropRegion = this.CropRegion
            };
        }

        /// <summary>
        /// Determines whether the options contains resizing.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// <c>true</c> if this instance contains resizing; otherwise, <c>false</c>.
        /// 
        /// </returns>
        public bool ContainsResizing()
        {
            if (this.Size.IsEmpty && this.MaxSize.IsEmpty && this.CropRegion == null)
                return (double)this.Scale > 0.0;
            else
                return true;
        }

        /// <summary>
        /// Determines whether this instance contains cropping.
        /// </summary>
        /// <returns></returns>
        internal bool ContainsCropping()
        {
            return this.CropRegion != null;
        }
    }
}
