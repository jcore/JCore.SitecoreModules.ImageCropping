using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources.Media;
using Sitecore.Sites;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore;

namespace JCore.SitecoreModules.ImageCropping.Resources.Media
{
    /// <summary>
    /// Represents a Media Query String.
    /// 
    /// </summary>
    public class CustomMediaUrlOptions : MediaUrlOptions
    {
        /// <summary>
        /// Gets or sets the crop region.
        /// </summary>
        /// <value>
        /// The crop region.
        /// </value>
        public string CropRegion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Resources.Media.MediaUrlOptions"/> class.
        /// 
        /// </summary>
        public CustomMediaUrlOptions() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Resources.Media.MediaUrlOptions"/> class.
        /// 
        /// </summary>
        /// <param name="width">The width.
        ///             </param><param name="height">The height.
        ///             </param><param name="thumbnail">if set to <c>true</c> this instance is thumbnail.
        ///             </param>
        public CustomMediaUrlOptions(int width, int height, bool thumbnail) : base(width,height, thumbnail)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Resources.Media.MediaUrlOptions"/> class.
        /// 
        /// </summary>
        /// <param name="database">The database.
        ///             </param>
        public CustomMediaUrlOptions(Database database) : base(database)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Resources.Media.MediaUrlOptions"/> class.
        /// 
        /// </summary>
        /// <param name="database">The database.
        ///             </param><param name="thumbnail">if set to <c>true</c> this instance is thumbnail.
        ///             </param>
        public CustomMediaUrlOptions(Database database, bool thumbnail) : base (database,thumbnail)
        {
        }

        /// <summary>
        /// Gets the thumbnail options.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public new static CustomMediaUrlOptions GetMediaOptions(MediaItem item)
        {
            Assert.ArgumentNotNull((object)item, "item");
            CustomMediaUrlOptions shellOptions = CustomMediaUrlOptions.GetShellOptions();
            shellOptions.Database = item.Database;
            shellOptions.DisableBrowserCache = true;
            shellOptions.UseDefaultIcon = true;
            shellOptions.BackgroundColor = Color.White;
            shellOptions.ItemRevision = item.InnerItem[FieldIDs.Revision];
            shellOptions.Language = item.InnerItem.Language;
            return shellOptions;
        }

        /// <summary>
        /// Gets the shell options.
        /// </summary>
        /// <returns></returns>
        public new static CustomMediaUrlOptions GetShellOptions()
        {
            return new CustomMediaUrlOptions()
            {
                AbsolutePath = false,
                RequestExtension = "ashx",
                UseItemPath = false,
                AlwaysIncludeServerUrl = false
            };
        }
        /// <summary>
        /// Parses the query string.
        /// 
        /// </summary>
        /// <param name="httpRequest">The http Request.
        ///             </param>
        public new void ParseQueryString(HttpRequest httpRequest)
        {
            Assert.ArgumentNotNull((object)httpRequest, "httpRequest");
            string str1 = httpRequest.QueryString["as"];
            if (!string.IsNullOrEmpty(str1))
                this.AllowStretch = MainUtil.GetBool(str1, false);
            string color = httpRequest.QueryString["bc"];
            if (!string.IsNullOrEmpty(color))
                this.BackgroundColor = MainUtil.StringToColor(color);
            try
            {
                this.Database = this.GetDatabase(httpRequest);
            }
            catch
            {
                HttpContext.Current.Response.Redirect(Settings.ItemNotFoundUrl);
            }
            string str2 = httpRequest.QueryString["dmc"];
            if (!string.IsNullOrEmpty(str2))
                this.DisableMediaCache = MainUtil.GetBool(str2, false);
            this.Height = MainUtil.GetInt(httpRequest.QueryString["h"], 0);
            string str3 = httpRequest.QueryString["iar"];
            if (!string.IsNullOrEmpty(str3))
                this.IgnoreAspectRatio = MainUtil.GetBool(str3, false);
            this.Language = this.GetLanguage(httpRequest);
            this.MaxHeight = MainUtil.GetInt(httpRequest.QueryString["mh"], 0);
            this.MaxWidth = MainUtil.GetInt(httpRequest.QueryString["mw"], 0);
            this.Scale = MainUtil.GetFloat(httpRequest.QueryString["sc"], 0.0f);
            string str4 = httpRequest.QueryString["thn"];
            if (!string.IsNullOrEmpty(str4))
                this.Thumbnail = MainUtil.GetBool(str4, false);
            this.Version = this.GetVersion(httpRequest);
            this.Width = MainUtil.GetInt(httpRequest.QueryString["w"], 0);

            // custom crop region
            var cropRegion = httpRequest.QueryString["cropregion"];
            if (!string.IsNullOrEmpty(cropRegion))
            {
                this.CropRegion = cropRegion;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// 
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            UrlString urlString = new UrlString(base.ToString());
            if (!string.IsNullOrEmpty(this.CropRegion))
                urlString.Add("cropregion", this.CropRegion);
            return urlString.ToString();
        }
    }
}
