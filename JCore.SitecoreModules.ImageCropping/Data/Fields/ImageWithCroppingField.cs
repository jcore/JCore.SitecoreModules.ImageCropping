using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Fields;

namespace XCore.SitecoreModules.ImageCropping.Data.Fields
{
    public class ImageWithCroppingField : ImageField
    {
        public ImageWithCroppingField(Field innerField) : base(innerField)
        {
        }

        public ImageWithCroppingField(Field innerField, string runtimeValue) : base(innerField, runtimeValue)
        {
        }

        public string CroppRegion
        {
            get
            {
                return GetAttribute("cropregion");
            }
            set
            {
                SetAttribute("alt", value);
            }
        }

        public string Ratio
        {
            get
            {
                return GetAttribute("ratio");
            }
            set
            {
                SetAttribute("ratio", value.ToString());
            }
        }
    }
}
