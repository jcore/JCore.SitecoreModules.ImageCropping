using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Configuration;
using Sitecore.ImageLib;

namespace Sitecore.SharedModules.ImageCropping.Resources.Media
{
    /// <summary>
    /// ResizeOptions class
    /// 
    /// </summary>
    public class CropOptions : ResizeOptions
    {
        /// <summary>
        /// Gets or sets the crop coordinates.
        /// </summary>
        /// <value>
        /// The crop coordinates.
        /// </value>
        public IDictionary<string, int> CropCoordinates { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is empty (i.e. <see cref="P:Sitecore.ImageLib.ResizeOptions.Size"/> is empty and <see cref="P:Sitecore.ImageLib.ResizeOptions.Scale"/> is 0).
        /// 
        /// </summary>
        /// 
        /// <value>
        /// <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                if (this.Size.IsEmpty && this.MaxSize.IsEmpty)
                    return (double)this.Scale == 0.0;
                else
                    return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.ImageLib.ResizeOptions"/> class.
        /// 
        /// </summary>
        public CropOptions()
        {
            this.CompositingMode = Settings.Media.Resizing.CompositingMode;
            this.PixelOffsetMode = Settings.Media.Resizing.PixelOffsetMode;
            this.InterpolationMode = Settings.Media.InterpolationMode;
        }
    }
}
