using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCore.SitecoreModules.ImageCropping.Resources.Media;
using Sitecore.Collections;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web;
using Sitecore.Xml.Xsl;

namespace JCore.SitecoreModules.ImageCropping.Pipelines.RenderField
{
    /// <summary>
    /// Implements the RenderField.
    /// 
    /// </summary>
    public class GetImageWithCroppingValue : GetImageFieldValue
    {
        /// <summary>
        /// Gets the field value.
        /// 
        /// </summary>
        /// <param name="args">The arguments.</param><contract><requires name="args" condition="none"/></contract>
        public new void Process(RenderFieldArgs args)
        {
            if (args.FieldTypeKey != "image with cropping")
                return;
            ImageRenderer renderer = CreateRenderer();
            renderer.Item = args.Item;
            renderer.FieldName = args.FieldName;

            XmlValue xmlValue = new XmlValue(args.FieldValue, "image");
            string width = xmlValue.GetAttribute("width");
            string height = xmlValue.GetAttribute("height");
            if (string.IsNullOrWhiteSpace(width) || string.IsNullOrWhiteSpace(height))
            {
                string cropRegion = xmlValue.GetAttribute("cropregion");
                try
                {
                    var coordinates = CustomMediaManager.ConvertToIntArray(cropRegion);
                    if ((coordinates[2] - coordinates[0] + coordinates[3] + coordinates[1]) > 0)
                    {
                        xmlValue.SetAttribute("width", (coordinates[2] - coordinates[0]).ToString());
                        xmlValue.SetAttribute("height", (coordinates[3] - coordinates[1]).ToString());
                        renderer.FieldValue = xmlValue.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex, this);
                }
            }
            if (renderer.FieldValue == null)
            {
                renderer.FieldValue = args.FieldValue;
            }
            
            renderer.Parameters = args.Parameters;

            args.WebEditParameters.AddRange(args.Parameters);
            RenderFieldResult renderFieldResult = renderer.Render();
            args.Result.FirstPart = renderFieldResult.FirstPart;
            args.Result.LastPart = renderFieldResult.LastPart;
            args.DisableWebEditContentEditing = true;
            args.DisableWebEditFieldWrapping = true;
            args.WebEditClick = "return Sitecore.WebEdit.editControl($JavascriptParameters, 'webedit:chooseimage')";
        }

        /// <summary>
        /// Creates the renderer.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The renderer.
        /// </returns>
        protected override ImageRenderer CreateRenderer()
        {
            return new ImageWithCropRenderer();
        }

    }
}
