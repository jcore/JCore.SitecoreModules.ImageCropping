using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Install.Files;
using Sitecore.Resources.Media;
using Sitecore.Sites;
using Sitecore.Text;
using Sitecore.Xml.Xsl;

namespace JCore.SitecoreModules.ImageCropping
{
    /// <summary>
    /// Implements the Image Renderer.
    /// 
    /// </summary>
    public class ImageWithCropRenderer : Sitecore.Xml.Xsl.ImageRenderer
    {
        private string cropRegion;
        private bool cropSet;
        /// <summary>
        /// Gets the value of source attribute taking all parameters into account.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The source.
        /// 
        /// </returns>
        protected override string GetSource()
        {
            var imgSrc = base.GetSource();
            if (this.cropSet)
            {
                imgSrc = string.Format("{0}{1}cropregion={2}",imgSrc,(imgSrc.Contains("?") ? "&" : "?"), this.cropRegion);
            }
            return imgSrc;
        }


        /// <summary>
        /// Parses the field.
        /// 
        /// </summary>
        /// <param name="imageFieldParse">The image field.
        ///             </param>
        protected override void ParseField(ImageField imageFieldParse)
        {
            Assert.ArgumentNotNull((object)imageFieldParse, "imageFieldParse");
            base.ParseField(imageFieldParse);
            if (string.IsNullOrEmpty(this.cropRegion))
            {
                this.cropRegion = StringUtil.GetString(imageFieldParse.GetAttribute("cropregion"), string.Empty);
                this.cropSet = true;
            }
        }
        /// <summary>
        /// Extracts a value from a dictionary and removes it. Alsow modifies flag wherever value was setted.
        /// 
        /// </summary>
        /// <param name="values">The values.
        ///             </param><param name="valueSet">if set to <c>true</c> this instance is value was setted.
        ///             </param><param name="keys">The keys to extract.
        ///             </param>
        /// <returns>
        /// The extracted key as string.
        /// 
        /// </returns>
        private string Extract(SafeDictionary<string> values, ref bool valueSet, params string[] keys)
        {
            Assert.ArgumentNotNull((object)values, "values");
            Assert.ArgumentNotNull((object)keys, "keys");
            string str = this.Extract(values, keys);
            valueSet = str != null;
            return str;
        }
    }
}
