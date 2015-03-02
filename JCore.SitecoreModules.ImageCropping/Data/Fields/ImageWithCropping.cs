using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using JCore.SitecoreModules.ImageCropping.Models;
using JCore.SitecoreModules.ImageCropping.Resources.Media;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Resources.Media;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.Dialogs.MediaBrowser;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using Sitecore.Exceptions;

namespace JCore.SitecoreModules.ImageCropping.Data.Fields
{
    /// <summary>
    /// Represents an Image field.
    /// 
    /// </summary>
    public class ImageWithCropping : Image
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Shell.Applications.ContentEditor.Image"/> class.
        /// 
        /// </summary>
        public ImageWithCropping() : base()
        {
        }

        /// <summary>
        /// Renders the control.
        /// 
        /// </summary>
        /// <param name="output">The output.</param>
        protected override void DoRender(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull((object)output, "output");
            Item mediaItem = this.GetMediaItem();
            string src;
            this.GetSrc(out src);
            string str1 = " src=\"" + src + "\"";
            string str2 = " id=\"" + this.ID + "_image\"";
            string str3 = " alt=\"" + (mediaItem != null ? WebUtil.HtmlEncode(mediaItem["Alt"]) : string.Empty) + "\"";
            //base.DoRender(output);
            output.Write("<div id=\"" + this.ID + "_pane\" class=\"scContentControlImagePane\">");
            string clientEvent = Sitecore.Context.ClientPage.GetClientEvent(this.ID + ".Browse");
            output.Write("<div class=\"scContentControlImageImage\" onclick=\"" + clientEvent + "\">");
            output.Write("<iframe" + str2 + str1 + str3 + " frameborder=\"0\" marginwidth=\"0\" marginheight=\"0\" width=\"100%\" height=\"128\" allowtransparency=\"allowtransparency\"></iframe>");
            output.Write("</div>");
            output.Write("<div id=\"" + this.ID + "_details\" class=\"scContentControlImageDetails\">");
            string details = this.GetDetails();
            output.Write(details);
            output.Write("</div>");
            output.Write("</div>");
        }

        /// <summary>
        /// Shows the properties.
        /// 
        /// </summary>
        /// <param name="args">The args.</param>
        protected new void ShowProperties(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            if (this.Disabled)
                return;
            string attribute = this.XmlValue.GetAttribute("mediaid");
            if (string.IsNullOrEmpty(attribute))
                SheerResponse.Alert("Select an image from the Media Library first.", new string[0]);
            else if (args.IsPostBack)
            {
                if (!args.HasResult)
                    return;
                this.XmlValue = new XmlValue(args.Result, "image");
                this.Value = this.XmlValue.GetAttribute("mediapath");
                this.SetModified();
                this.Update();
            }
            else
            {
                UrlString urlString = new UrlString("/sitecore/shell/~/xaml/Sitecore.Shell.Applications.Media.ExtendedImageProperties.aspx");
                Item obj = Client.ContentDatabase.GetItem(attribute, Language.Parse(this.ItemLanguage));
                if (obj == null)
                {
                    SheerResponse.Alert("Select an image from the Media Library first.", new string[0]);
                }
                else
                {
                    obj.Uri.AddToUrlString(urlString);
                    UrlHandle urlHandle = new UrlHandle();
                    urlHandle["xmlvalue"] = this.XmlValue.ToString();
                    urlHandle.Add(urlString);
                    SheerResponse.ShowModalDialog(urlString.ToString(), "700", "700", string.Empty, true);
                    args.WaitForPostBack();
                }
            }
        }

        /// <summary>
        /// Handles the Change event.
        /// 
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void DoChange(Message message)
        {
            Assert.ArgumentNotNull((object)message, "message");
            base.DoChange(message);
            if (string.IsNullOrEmpty(this.Value))
            {
                this.ClearImage();
            }
            else
            {
                this.XmlValue.SetAttribute("mediapath", this.Value);
                string path = this.Value;
                if (!path.StartsWith("/sitecore", StringComparison.InvariantCulture))
                    path = "/sitecore/media library" + path;
                MediaItem mediaItem = (MediaItem)Client.ContentDatabase.GetItem(path);
                if (mediaItem != null)
                {
                    string str = mediaItem.InnerItem.Paths.Path;
                    if (str.StartsWith("/sitecore/media library", StringComparison.InvariantCulture))
                        str = str.Substring("/sitecore/media library".Length);
                    MediaUrlOptions shellOptions = MediaUrlOptions.GetShellOptions();
                    string mediaUrl = MediaManager.GetMediaUrl(mediaItem, shellOptions);
                    this.XmlValue.SetAttribute("mediaid", mediaItem.ID.ToString());
                    this.XmlValue.SetAttribute("mediapath", str);
                    this.XmlValue.SetAttribute("src", Images.GetUncachedImageSrc(mediaUrl));
                }
                else
                {
                    this.XmlValue.SetAttribute("mediaid", string.Empty);
                    this.XmlValue.SetAttribute("src", string.Empty);
                }
                this.Update();
                this.SetModified();
            }
            SheerResponse.SetReturnValue(true);
        }

        /// <summary>
        /// Updates this instance.
        /// 
        /// </summary>
        protected new void Update()
        {
            string src;
            this.GetSrc(out src);
            SheerResponse.SetAttribute(this.ID + "_image", "src", src);
            SheerResponse.SetInnerHtml(this.ID + "_details", this.GetDetails());
            SheerResponse.Eval("scContent.startValidators()");
        }

        protected new void Browse()
        {
            if (this.Disabled)
                return;
            Sitecore.Context.ClientPage.Start(this, "BrowseImage");
        }


        /// <summary>
        /// Browses for an image.
        /// 
        /// </summary>
        /// <param name="args">The args.</param><exception cref="T:Sitecore.Exceptions.ClientAlertException">The source of this Image field points to an item that does not exist.</exception>
        protected new void BrowseImage(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.IsPostBack)
            {
                if (string.IsNullOrEmpty(args.Result) || args.Result == "undefined")
                    return;
                MediaItem mediaItem = Client.ContentDatabase.Items[args.Result];
                if (mediaItem != null)
                {
                    TemplateItem template = mediaItem.InnerItem.Template;
                    if (template != null && !IsImageMedia(template))
                    {
                        SheerResponse.Alert("The selected item does not contain an image.");
                    }
                    else
                    {
                        this.XmlValue.SetAttribute("mediaid", mediaItem.ID.ToString());
                        var croppingOption = GetCroppingOption();
                        var cropRegion = ProduceCropRegionXmlAttributeValueFromAnOption(mediaItem, croppingOption);
                        if (!string.IsNullOrWhiteSpace(cropRegion))
                        {
                            this.XmlValue.SetAttribute("cropregion", cropRegion);
                        }
                        this.XmlValue.SetAttribute("width", croppingOption.Width.ToString());
                        this.XmlValue.SetAttribute("height", croppingOption.Height.ToString());
                        this.Value = mediaItem.MediaPath;
                        this.Update();
                        this.SetModified();
                    }
                }
                else
                    SheerResponse.Alert("Item not found.");
            }
            else
            {
                var souceValues = HttpUtility.ParseQueryString(Source);
                var mediaSource = string.Empty;
                if (souceValues.AllKeys.Any())
                {
                    mediaSource = souceValues["mediaSource"];
                }
                string str1 = StringUtil.GetString(new string[2]
                {
                  mediaSource,
                  "/sitecore/media library"
                });
                string str2 = str1;
                string path = this.XmlValue.GetAttribute("mediaid");
                string str3 = path;
                if (str1.StartsWith("~", StringComparison.InvariantCulture))
                {
                    str2 = StringUtil.Mid(str1, 1);
                    if (string.IsNullOrEmpty(path))
                        path = str2;
                    str1 = "/sitecore/media library";
                }
                Language language = Language.Parse(this.ItemLanguage);
                MediaBrowserOptions mediaBrowserOptions = new MediaBrowserOptions();
                Item obj1 = Client.ContentDatabase.GetItem(str1, language);
                if (obj1 == null)
                    throw new ClientAlertException("The source of this Image field points to an item that does not exist.");
                mediaBrowserOptions.Root = obj1;
                if (!string.IsNullOrEmpty(path))
                {
                    Item obj2 = Client.ContentDatabase.GetItem(path, language);
                    if (obj2 != null)
                        mediaBrowserOptions.SelectedItem = obj2;
                }
                UrlHandle urlHandle = new UrlHandle();
                urlHandle["ro"] = str1;
                urlHandle["fo"] = str2;
                urlHandle["db"] = Client.ContentDatabase.Name;
                urlHandle["la"] = this.ItemLanguage;
                urlHandle["va"] = str3;
                UrlString urlString = mediaBrowserOptions.ToUrlString();
                urlHandle.Add(urlString);
                SheerResponse.ShowModalDialog(urlString.ToString(), "1200px", "700px", string.Empty, true);
                args.WaitForPostBack();
            }
        }

        private string ProduceCropRegionXmlAttributeValueFromAnOption(MediaItem mediaItem, CroppingOption croppingOption)
        {
            Assert.ArgumentNotNull(mediaItem, "mediaItem");
            Assert.ArgumentNotNull(croppingOption, "croppingOption");

            if (croppingOption.CroppingRegionHorizontalAlignment == HorizonatalAlignment.Undefined ||
                croppingOption.CroppingRegionVerticalAlignment == VerticalAlignment.Undefined)
            {
                return string.Empty;
            }

            var originalWidth = Int32.Parse(mediaItem.InnerItem["Width"]);
            var originalHeight = Int32.Parse(mediaItem.InnerItem["Height"]);

            int x1 = 0;
            int x2 = originalWidth;

            // x1 and x2 coordinates
            if (croppingOption.CroppingRegionHorizontalAlignment == HorizonatalAlignment.Center &&
                originalWidth > croppingOption.Width)
            {
                x1 = (originalWidth - croppingOption.Width)/2;
            }
            else if (croppingOption.CroppingRegionHorizontalAlignment == HorizonatalAlignment.Right &&
                     originalWidth > croppingOption.Width)
            {
                x1 = (originalWidth - croppingOption.Width);
            }
            x2 = croppingOption.Width + x1;

            int y1 = 0;
            int y2 = originalHeight;
            if (croppingOption.CroppingRegionVerticalAlignment == VerticalAlignment.Middle &&
                originalHeight > croppingOption.Height)
            {
                y1 = (originalHeight - croppingOption.Height) / 2;
            }
            else if (croppingOption.CroppingRegionVerticalAlignment == VerticalAlignment.Bottom &&
                originalHeight > croppingOption.Height)
            {
                y1 = originalHeight - croppingOption.Height;
            }
            y2 = croppingOption.Height + y1;

            if (x1 == 0 && x2 == originalWidth && y1 == 0 && y2 == originalHeight)
            {
                return string.Empty;
            }
            return string.Join(",", new[] { x1, y1, x2, y2 });
        }


        public virtual string CroppingOption
        {
            get
            {
                return GetViewStateString("CroppingOption");
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                string str = MainUtil.UnmapPath(value);
                if (str.EndsWith("/", StringComparison.InvariantCulture))
                    str = str.Substring(0, str.Length - 1);
                SetViewStateString("CroppingOption", str);
            }
        }

        /// <summary>
        /// Clears the image.
        /// 
        /// </summary>
        private void ClearImage()
        {
            if (this.Disabled)
                return;
            if (this.Value.Length > 0)
                this.SetModified();
            this.XmlValue = new XmlValue(string.Empty, "image");
            this.Value = string.Empty;
            this.Update();
        }
        /// <summary>
        /// Gets the image source.
        /// 
        /// </summary>
        /// <param name="src">The image source.</param>
        private void GetSrc(out string src)
        {
            src = string.Empty;
            MediaItem mediaItem = GetMediaItem();
            if (mediaItem == null)
                return;
            var thumbnailOptions = CustomMediaUrlOptions.GetMediaUrlOptions(mediaItem);
            int result;
            if (!int.TryParse(mediaItem.InnerItem["Height"], out result))
                result = 128;
            thumbnailOptions.Height = Math.Min(128, result);
            thumbnailOptions.MaxWidth = 640;
            thumbnailOptions.UseDefaultIcon = true;

            XmlValue xmlValue = XmlValue;
            string cropRegion = WebUtil.HtmlEncode(xmlValue.GetAttribute("cropregion"));
            if (!string.IsNullOrEmpty(cropRegion))
            {
                thumbnailOptions.CropRegion = cropRegion;
            }
            src = CustomMediaManager.GetMediaUrl(mediaItem, thumbnailOptions);
        }

        /// <summary>
        /// Gets the details.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The details.
        /// </returns>
        /// <contract><ensures condition="not null"/></contract>
        private string GetDetails()
        {
            string str1 = string.Empty;
            var croppingOption = GetCroppingOption();
            var croppingOptionString = string.Empty;
            if (croppingOption != null)
            {
                croppingOptionString = String.Format("<div>Default Cropping: {0} - {1}x{2}</div>", croppingOption.Name, croppingOption.Width, croppingOption.Height);
            }
            MediaItem mediaItem = (MediaItem)this.GetMediaItem();
            if (mediaItem != null)
            {
                Item innerItem = mediaItem.InnerItem;
                StringBuilder stringBuilder = new StringBuilder();
                XmlValue xmlValue = this.XmlValue;
                stringBuilder.Append("<div>");
                string str2 = innerItem["Dimensions"];
                string str3 = WebUtil.HtmlEncode(xmlValue.GetAttribute("width"));
                string str4 = WebUtil.HtmlEncode(xmlValue.GetAttribute("height"));
                string cropRegion = WebUtil.HtmlEncode(xmlValue.GetAttribute("cropregion"));
                if (!string.IsNullOrEmpty(cropRegion))
                {
                    var cropRegionArr = cropRegion.Split(',');                  
                    var croppedWidth = Math.Round((decimal)ConvertToInt(cropRegionArr[2]) - ConvertToInt(cropRegionArr[0]));
                    var croppedHeight = Math.Round((decimal)ConvertToInt(cropRegionArr[3]) - ConvertToInt(cropRegionArr[1]));

                    if (string.IsNullOrEmpty(str3) || string.IsNullOrEmpty(str4))
                    {
                        str3 = croppedWidth.ToString(CultureInfo.InvariantCulture);
                        str4 = croppedHeight.ToString(CultureInfo.InvariantCulture);
                    }
                    stringBuilder.Append(Translate.Text("Dimensions: {0} x {1} (Cropped. Original: {2})", (object)str3, (object)str4, (object)str2));
                }
                else
                {
                    if (!string.IsNullOrEmpty(str3) || !string.IsNullOrEmpty(str4))
                    {
                        stringBuilder.Append(Translate.Text("Dimensions: {0} x {1} (Original: {2})", (object)str3, (object)str4, (object)str2));
                    }
                    else
                    {
                        stringBuilder.Append(Translate.Text("Dimensions: {0}", new object[1]
                      {
                        (object) str2
                      }));
                    }
                }
                stringBuilder.Append("</div>");
                stringBuilder.Append("<div style=\"padding:2px 0px 0px 0px\">");
                string str5 = WebUtil.HtmlEncode(innerItem["Alt"]);
                string str6 = WebUtil.HtmlEncode(xmlValue.GetAttribute("alt"));
                if (!string.IsNullOrEmpty(str6) && !string.IsNullOrEmpty(str5))
                    stringBuilder.Append(Translate.Text("Alternate Text: \"{0}\" (Default Alternate Text: \"{1}\")", (object)str6, (object)str5));
                else if (!string.IsNullOrEmpty(str6))
                    stringBuilder.Append(Translate.Text("Alternate Text: \"{0}\"", new object[1]
          {
            (object) str6
          }));
                else if (!string.IsNullOrEmpty(str5))
                    stringBuilder.Append(Translate.Text("Default Alternate Text: \"{0}\"", new object[1]
          {
            (object) str5
          }));
                else
                    stringBuilder.Append(Translate.Text("Warning: Alternate Text is missing."));
                stringBuilder.Append("</div>");

                if (!string.IsNullOrWhiteSpace(croppingOptionString))
                {
                    stringBuilder.Append(croppingOptionString);
                }
                str1 = ((object)stringBuilder).ToString();
            }

            if (str1.Length == 0)
            {
                str1 = Translate.Text("This media item has no details.");
                if (!string.IsNullOrWhiteSpace(croppingOptionString))
                {
                    str1 += croppingOptionString;
                }
            }
            return str1;
        }

        private CroppingOption GetCroppingOption()
        {
            if (!string.IsNullOrWhiteSpace(Source))
            {
                var values = HttpUtility.ParseQueryString(Source);
                if (values.HasKeys())
                {
                    var croppingOptionSource = values["CroppingOption"];
                    if (!string.IsNullOrWhiteSpace(croppingOptionSource))
                    {
                        Language language = Language.Parse(ItemLanguage);
                        Item croppingOptionItem = Client.ContentDatabase.GetItem(croppingOptionSource, language);
                        if (croppingOptionItem != null)
                        {
                            return new CroppingOption
                            {
                                Name = croppingOptionItem.DisplayName,
                                Width = Int32.Parse(croppingOptionItem.Fields["Width"].Value),
                                Height = Int32.Parse(croppingOptionItem.Fields["Height"].Value),
                                CroppingRegionHorizontalAlignment = GetCroppingRegionHorizontalAlignment(croppingOptionItem),
                                CroppingRegionVerticalAlignment = GetCroppingRegionVerticalAlignment(croppingOptionItem)
                            };
                        }
                    }
                }
            }
            return null;
        }

        private static VerticalAlignment GetCroppingRegionVerticalAlignment(Item croppingOptionItem)
        {
            if (croppingOptionItem != null &&
                !string.IsNullOrWhiteSpace(croppingOptionItem["Cropping Region Vertical Alignment"]))
            {
                {
                    return
                        (VerticalAlignment)
                            Enum.Parse(typeof (VerticalAlignment),
                                croppingOptionItem.Fields["Cropping Region Vertical Alignment"].Value);
                }
            }
            return VerticalAlignment.Undefined;
        }

        /// <summary>
        /// Gets the cropping region horizontal alignment.
        /// </summary>
        /// <param name="croppingOptionItem">The cropping option item.</param>
        /// <returns></returns>
        private static HorizonatalAlignment GetCroppingRegionHorizontalAlignment(Item croppingOptionItem)
        {
            if (croppingOptionItem != null &&
                !string.IsNullOrWhiteSpace(croppingOptionItem["Cropping Region Horizontal Alignment"]))
            {
                return
                    (HorizonatalAlignment)
                        Enum.Parse(typeof (HorizonatalAlignment),
                            croppingOptionItem.Fields["Cropping Region Horizontal Alignment"].Value);
            }
            return  HorizonatalAlignment.Undefined;
        }

        /// <summary>
        /// Converts to int.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        private int ConvertToInt(string value)
        {
            var intValue = 0;
            if (!int.TryParse(value, out intValue))
            {
                intValue = 0;
            }
            return intValue;
        }

        /// <summary>
        /// Gets the media item.
        /// </summary>  
        /// <returns>
        /// The media item.
        /// </returns>
        private Item GetMediaItem()
        {
            string attribute = XmlValue.GetAttribute("mediaid");
            if (attribute.Length <= 0)
                return null;
            Language language = Language.Parse(ItemLanguage);
            return Client.ContentDatabase.GetItem(attribute, language);
        }

        private bool IsImageMedia(TemplateItem template)
        {
            Assert.ArgumentNotNull(template, "template");
            if (template.ID == TemplateIDs.VersionedImage || template.ID == TemplateIDs.UnversionedImage)
                return true;
            return template.BaseTemplates.Any(IsImageMedia);
        }
     }
}
