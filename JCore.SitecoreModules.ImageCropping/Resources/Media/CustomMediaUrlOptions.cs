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
        private static CustomMediaUrlOptions _empty;
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
        public CustomMediaUrlOptions()
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

        public CustomMediaUrlOptions(int width, int height, string cropregion, bool thumbnail)
            : base(width, height, thumbnail)
        {
            CropRegion = cropregion;
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
        public static CustomMediaUrlOptions GetMediaUrlOptions(MediaItem item)
        {
            Assert.ArgumentNotNull(item, "item");
            CustomMediaUrlOptions shellOptions = GetShellOptions();
            shellOptions.Database = item.Database;
            shellOptions.DisableBrowserCache = true;
            shellOptions.UseDefaultIcon = true;
            shellOptions.BackgroundColor = Color.White;
            shellOptions.ItemRevision = item.InnerItem[FieldIDs.Revision];
            shellOptions.Language = item.InnerItem.Language;
            return shellOptions;
        }

        public new static CustomMediaUrlOptions GetThumbnailOptions(MediaItem item)
        {
            Assert.ArgumentNotNull((object)item, "item");
            CustomMediaUrlOptions shellOptions = GetShellOptions();
            shellOptions.Database = item.Database;
            shellOptions.Thumbnail = true;
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
            return new CustomMediaUrlOptions
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
            base.ParseQueryString(httpRequest);

            // custom crop region
            var cropRegion = httpRequest.QueryString["cropregion"];
            if (!string.IsNullOrEmpty(cropRegion))
            {
                CropRegion = cropRegion;
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
            var urlString = new UrlString(base.ToString());
            return !string.IsNullOrEmpty(CropRegion) ? string.Format("{0}{1}cropregion={2}", urlString, (urlString.Parameters != null && urlString.Parameters.Count > 0 ? "&" : "?"), CropRegion) : urlString.ToString();
        }

        public new static CustomMediaUrlOptions Empty
        {
            get
            {
                return _empty ?? new CustomMediaUrlOptions();
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                _empty = value;
            }
        }

    }
}
