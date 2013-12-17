Sitecore.ImageCropping
======================

Image cropping functionality that allows specifying cropping region when assigning asset to Image field.

This module includes a custom image field and croping processor.

Image Field:

- Allows editors to specify crop region for an image.
- Saves the crop region into image field xml


Crop Processor:

- Processor for getMediaStream pipeline that crops an image based on specified crop region ("cropregion" query string parameter in "x1,y1,x2,y2" format).


Example:

There are 3 different ways you can use this module:
 - &lt;sc:image field="image" /&gt;
 - Sitecore.Web.UI.WebControls.FieldRenderer.Render(Sitecore.Context.Item, "Image");
 - CustomMediaManager.GetMediaUrl(((Sitecore.Data.Fields.ImageField)Sitecore.Context.Item.Fields["Image"]));

