using Sitecore;
using Sitecore.Collections;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Xml.Xsl;
using JCore.SitecoreModules.ImageCropping.Data.Fields;

namespace JCore.SitecoreModules.ImageCropping
{
    /// <summary>
    /// Implements the Image Renderer.
    /// 
    /// </summary>
    public class ImageWithCropRenderer : ImageRenderer
    {
        private string _cropRegion;
        private bool _cropSet;
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
            if (_cropSet)
            {
                imgSrc = string.Format("{0}{1}cropregion={2}",imgSrc,(imgSrc.Contains("?") ? "&" : "?"), _cropRegion);
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
            Assert.ArgumentNotNull(imageFieldParse, "imageFieldParse");
            base.ParseField(imageFieldParse);
            if (string.IsNullOrEmpty(_cropRegion))
            {
                if (imageFieldParse != null)
                    _cropRegion = StringUtil.GetString(imageFieldParse.GetAttribute("cropregion"), string.Empty);
                _cropSet = true;
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
            Assert.ArgumentNotNull(values, "values");
            Assert.ArgumentNotNull(keys, "keys");
            var str = Extract(values, keys);
            valueSet = str != null;
            return str;
        }
    }
}
