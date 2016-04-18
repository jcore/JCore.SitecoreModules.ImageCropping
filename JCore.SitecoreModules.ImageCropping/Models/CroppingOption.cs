using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCore.SitecoreModules.ImageCropping.Models
{
    public class CroppingOption
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public HorizonatalAlignment CroppingRegionHorizontalAlignment { get; set; }
        public VerticalAlignment CroppingRegionVerticalAlignment { get; set; }
    }

    public enum HorizonatalAlignment
    {
        Left, Center, Right, Undefined
    }
    public enum VerticalAlignment
    {
        Top, Middle, Bottom, Undefined
    }
}
