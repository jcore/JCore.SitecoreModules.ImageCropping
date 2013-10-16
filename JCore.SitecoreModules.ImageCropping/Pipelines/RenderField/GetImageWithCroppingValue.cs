using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Collections;
using Sitecore.Pipelines.RenderField;
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
        public void Process(RenderFieldArgs args)
        {
            if (args.FieldTypeKey != "image with cropping")
                return;
            ImageRenderer renderer = this.CreateRenderer();
            renderer.Item = args.Item;
            renderer.FieldName = args.FieldName;
            renderer.FieldValue = args.FieldValue;
            renderer.Parameters = args.Parameters;
            args.WebEditParameters.AddRange((SafeDictionary<string, string>)args.Parameters);
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
        protected virtual ImageRenderer CreateRenderer()
        {
            return new ImageWithCropRenderer();
        }

    }
}
