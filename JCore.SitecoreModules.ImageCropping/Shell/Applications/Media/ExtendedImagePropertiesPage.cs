using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Controls;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources.Media;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XamlSharp.Xaml;


namespace JCore.SitecoreModules.ImageCropping.Shell.Applications.Media
{
    public class ExtendedImagePropertiesPage : DialogPage
    {
        private const int imageHeight = 300;
        /// <summary>
        /// The alt.
        /// </summary>
        protected TextBox Alt;
        /// <summary>
        /// The aspect.
        /// </summary>
        protected global::Sitecore.Web.UI.HtmlControls.Checkbox Aspect;
        /// <summary>
        /// The h space.
        /// </summary>
        protected TextBox HSpace;
        /// <summary>
        /// The height edit.
        /// </summary>
        protected TextBox HeightEdit;
        /// <summary>
        /// The original size.
        /// </summary>
        protected global::Sitecore.Web.UI.HtmlControls.Literal OriginalSize;
        /// <summary>
        /// The original text.
        /// </summary>
        protected TextBox OriginalText;
        /// <summary>
        /// The size warning.
        /// </summary>
        protected Border SizeWarning;
        /// <summary>
        /// The v space.
        /// </summary>
        protected TextBox VSpace;
        /// <summary>
        /// The width edit.
        /// </summary>
        protected TextBox WidthEdit;

        protected HiddenField OriginalWidth;
        protected HiddenField OriginalHeight;

        /// <summary>
        /// The image
        /// </summary>
        protected System.Web.UI.WebControls.Image Img;

        protected TextBox X1;
        protected TextBox X2;
        protected TextBox Y1;
        protected TextBox Y2;

        /// <summary>
        /// Gets or sets the height of the image.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The height of the image.
        /// </value>
        public int ImageHeight
        {
            get
            {
                return (int)this.ViewState["ImageHeight"];
            }
            set
            {
                this.ViewState["ImageHeight"] = (object)value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the image.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The width of the image.
        /// </value>
        public int ImageWidth
        {
            get
            {
                return (int)this.ViewState["ImageWidth"];
            }
            set
            {
                this.ViewState["ImageWidth"] = (object)value;
            }
        }

        /// <summary>
        /// Gets or sets the XML value.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The XML value.
        /// </value>
        /// <contract><requires name="value" condition="not null"/><ensures condition="nullable"/></contract>
        private XmlValue XmlValue
        {
            get
            {
                return new XmlValue(StringUtil.GetString(this.ViewState["XmlValue"]), "image");
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.ViewState["XmlValue"] = (object)value.ToString();
            }
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        private Database Database
        {
            get
            {
                return global::Sitecore.Context.ContentDatabase ?? global::Sitecore.Context.Database;
            }
        }

        /// <summary>
        /// Changes the height.
        /// </summary>
        protected void ChangeHeight()
        {
            if (this.ImageHeight == 0)
                return;
            int num = MainUtil.GetInt(this.HeightEdit.Text, 0);
            if (num > 0)
            {
                if (num > 8192)
                {
                    num = 8192;
                    this.HeightEdit.Text = "8192";
                    SheerResponse.SetAttribute(this.HeightEdit.ClientID, "value", this.HeightEdit.Text);
                }
                if (this.Aspect.Checked)
                {
                    this.WidthEdit.Text = ((int)((double)num / (double)this.ImageHeight * (double)this.ImageWidth)).ToString();
                    SheerResponse.SetAttribute(this.WidthEdit.ClientID, "value", this.WidthEdit.Text);
                }
            }
            SheerResponse.SetReturnValue(true);
        }

        /// <summary>
        /// Changes the width.
        /// </summary>
        protected void ChangeWidth()
        {
            if (this.ImageWidth == 0)
                return;
            int num = MainUtil.GetInt(this.WidthEdit.Text, 0);
            if (num > 0)
            {
                if (num > 8192)
                {
                    num = 8192;
                    this.WidthEdit.Text = "8192";
                    SheerResponse.SetAttribute(this.WidthEdit.ClientID, "value", this.WidthEdit.Text);
                }
                if (this.Aspect.Checked)
                {
                    this.HeightEdit.Text = ((int)((double)num / (double)this.ImageWidth * (double)this.ImageHeight)).ToString();
                    SheerResponse.SetAttribute(this.HeightEdit.ClientID, "value", this.HeightEdit.Text);
                }
            }
            SheerResponse.SetReturnValue(true);
        }

        /// <summary>
        /// Handles a click on the OK button.
        /// </summary>
        /// 
        /// <remarks>
        /// When the user clicks OK, the dialog is closed by calling
        ///             the <see cref="M:Sitecore.Web.UI.Sheer.ClientResponse.CloseWindow">CloseWindow</see> method.
        /// </remarks>
        protected override void OK_Click()
        {
            XmlValue xmlValue = this.XmlValue;
            Assert.IsNotNull((object)xmlValue, "XmlValue");
            xmlValue.SetAttribute("alt", this.Alt.Text);
            xmlValue.SetAttribute("height", this.HeightEdit.Text);
            xmlValue.SetAttribute("width", this.WidthEdit.Text);
            xmlValue.SetAttribute("hspace", this.HSpace.Text);
            xmlValue.SetAttribute("vspace", this.VSpace.Text);
            var cropRegion = this.GenerateCropRegion();
            if (!string.IsNullOrEmpty(cropRegion))
            {
                xmlValue.SetAttribute("cropregion", cropRegion);
            }
            SheerResponse.SetDialogValue(xmlValue.ToString());
            base.OK_Click();
        }

        /// <summary>
        /// Generates the crop region.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private string GenerateCropRegion()
        {
            string[] coordinates = { this.X1.Text, this.Y1.Text, this.X2.Text, this.Y2.Text };
            return string.Join(",", coordinates);
            return string.Empty;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");
            base.OnLoad(e);
            if (XamlControl.AjaxScriptManager.IsEvent)
                return;
            this.ImageWidth = 0;
            this.ImageHeight = 0;
            ItemUri uri = ItemUri.ParseQueryString();
            if (uri == (ItemUri)null)
                return;
            Item obj = Database.GetItem(uri);
            if (obj == null)
                return;
            string text = obj["Dimensions"];
            if (string.IsNullOrEmpty(text))
                return;
            int length = text.IndexOf('x');
            if (length < 0)
                return;
            this.ImageWidth = MainUtil.GetInt(StringUtil.Left(text, length).Trim(), 0);
            this.ImageHeight = MainUtil.GetInt(StringUtil.Mid(text, length + 1).Trim(), 0);
            if (this.ImageWidth <= 0 || this.ImageHeight <= 0)
            {
                this.Aspect.Checked = false;
                this.Aspect.Disabled = true;
            }
            else
                this.Aspect.Checked = true;
            if (this.ImageWidth > 0)
                this.OriginalSize.Text = Translate.Text("Original Dimensions: {0} x {1}", (object)this.ImageWidth, (object)this.ImageHeight);
            if (MainUtil.GetLong((object)obj["Size"], 0L) >= Settings.Media.MaxSizeInMemory)
            {
                this.HeightEdit.Enabled = false;
                this.WidthEdit.Enabled = false;
                this.Aspect.Disabled = true;
            }
            else
            {
                this.SizeWarning.Visible = false;
            }
            this.OriginalText.Text = StringUtil.GetString(new string[2]
              {
                obj["Alt"],
                Translate.Text("[none]")
              });
            UrlHandle urlHandle = UrlHandle.Get();
            XmlValue xmlValue = new XmlValue(urlHandle["xmlvalue"], "image");
            this.XmlValue = xmlValue;
            this.Alt.Text = xmlValue.GetAttribute("alt");
            this.HeightEdit.Text = xmlValue.GetAttribute("height");
            this.WidthEdit.Text = xmlValue.GetAttribute("width");
            this.HSpace.Text = xmlValue.GetAttribute("hspace");
            this.VSpace.Text = xmlValue.GetAttribute("vspace");
            this.Img.ImageUrl = this.GetMediaUrl(obj);
            this.Img.Height = imageHeight;

            this.OriginalWidth.Value = obj["Width"];
            this.OriginalHeight.Value = obj["Height"];

            this.X1.Text = this.GetCoordinateValue(xmlValue, "x1");
            this.X2.Text = this.GetCoordinateValue(xmlValue, "x2");
            this.Y1.Text = this.GetCoordinateValue(xmlValue, "y1");
            this.Y2.Text = this.GetCoordinateValue(xmlValue, "y2");

            if (MainUtil.GetBool(urlHandle["disableheight"], false))
            {
                this.HeightEdit.Enabled = false;
                this.Aspect.Checked = false;
                this.Aspect.Disabled = true;
            }
            if (!MainUtil.GetBool(urlHandle["disablewidth"], false))
                return;
            this.WidthEdit.Enabled = false;
            this.Aspect.Checked = false;
            this.Aspect.Disabled = true;            
        }

        /// <summary>
        /// Gets the coordinate value.
        /// </summary>
        /// <param name="xmlValue">The XML value.</param>
        /// <param name="p">The application.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private string GetCoordinateValue(global::Sitecore.Shell.Applications.ContentEditor.XmlValue xmlValue, string coordinate)
        {
            var region = xmlValue.GetAttribute("cropregion");
            var coordinates = region.Split(',');
            if (coordinates.Count() == 4)
            {
                switch (coordinate)
                {
                    case "x1":
                        return coordinates[0];
                    case "x2":
                        return coordinates[2];
                    case "y1":
                        return coordinates[1];
                    case "y2":
                        return coordinates[3];
                    default:
                        return "0";
                }
            }
            return "0";
        }

        /// <summary>
        /// Gets the media URL.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private string GetMediaUrl(Item item)
        {
            return MediaManager.GetMediaUrl(item);
        }

        public string CropRegion { get; set; }
    }
}

