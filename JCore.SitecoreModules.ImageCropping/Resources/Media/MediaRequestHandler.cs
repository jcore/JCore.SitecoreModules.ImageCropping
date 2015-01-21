using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Resources.Media;
using Sitecore.Resources.Media.Streaming;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JCore.SitecoreModules.ImageCropping.Resources.Media
{
    public class CustomMediaRequestHandler : MediaRequestHandler
    {
        protected override Tristate Modified(HttpContext context, Sitecore.Resources.Media.Media media, MediaOptions options)
        {
            string str1 = context.Request.Headers["If-None-Match"];
            if (!string.IsNullOrEmpty(str1) && str1 != media.MediaData.MediaId)
                return Tristate.True;
            string str2 = context.Request.Headers["If-Modified-Since"];
            if (!string.IsNullOrEmpty(str2))
            {
                DateTime result;
                if (DateTime.TryParse(str2.Split(';')[0].Replace(" UTC", " GMT"), out result))
                    return MainUtil.GetTristate(!this.CompareDatesWithRounding(result, media.MediaData.Updated, new TimeSpan(0, 0, 1)));
                Log.Warn(string.Format("Can't parse header. The wrong value  - \"If-Modified-Since: {0}\" ", (object)str2), (object)typeof(MediaRequestHandler));
            }
            return Tristate.Undefined;
        }

        private bool CompareDatesWithRounding(DateTime firstData, DateTime secondDate, TimeSpan round)
        {
            return (firstData - secondDate).Duration() <= round;
        }

        protected override bool DoProcessRequest(HttpContext context, MediaRequest request, Sitecore.Resources.Media.Media media)
        {
            Assert.ArgumentNotNull((object)context, "context");
            Assert.ArgumentNotNull((object)request, "request");
            Assert.ArgumentNotNull((object)media, "media");
            if (this.Modified(context, media, request.Options) == Tristate.False)
            {
                Event.RaiseEvent("media:request", new object[1]
                {
                  (object) request
                });
                this.SendMediaHeaders(media, context);
                context.Response.StatusCode = 304;
                return true;
            }
            this.ProcessImageDimensions(request, media);
            MediaStream stream = media.GetStream(request.Options);
            if (stream == null)
                return false;
            Event.RaiseEvent("media:request", new object[1]
              {
                (object) request
              });
            if (Settings.Media.EnableRangeRetrievalRequest && Settings.Media.CachingEnabled)
            {
                using (stream)
                {
                    this.SendMediaHeaders(media, context);
                    new RangeRetrievalResponse(RangeRetrievalRequest.BuildRequest(context, media), stream).ExecuteRequest(context);
                    return true;
                }
            }
            else
            {
                this.SendMediaHeaders(media, context);
                this.SendStreamHeaders(stream, context);
                using (stream)
                {
                    context.Response.AddHeader("Content-Length", stream.Stream.Length.ToString());
                    WebUtil.TransmitStream(stream.Stream, context.Response, Settings.Media.StreamBufferSize);
                }
                return true;
            }
        }

        /// <summary>
        /// Processes dimensions for dynamically scaled images according to configuration.
        /// 
        /// </summary>
        /// <param name="request">The media request.</param><param name="media">The media.</param>
        private void ProcessImageDimensions(MediaRequest request, Sitecore.Resources.Media.Media media)
        {
            Assert.ArgumentNotNull((object)request, "request");
            Assert.ArgumentNotNull((object)media, "media");
            Item innerItem = media.MediaData.MediaItem.InnerItem;
            int result1;
            int.TryParse(innerItem["Height"], out result1);
            int result2;
            int.TryParse(innerItem["Width"], out result2);
            bool flag = false;
            int maxHeight = Settings.Media.Resizing.MaxHeight;
            if (maxHeight != 0 && request.Options.Height > Math.Max(maxHeight, result1))
            {
                flag = true;
                request.Options.Height = Math.Max(maxHeight, result1);
            }
            int maxWidth = Settings.Media.Resizing.MaxWidth;
            if (maxWidth != 0 && request.Options.Width > Math.Max(maxWidth, result2))
            {
                flag = true;
                request.Options.Width = Math.Max(maxWidth, result2);
            }
            if (!flag)
                return;
            Log.Warn(string.Format("Requested image exceeds allowed size limits. Requested URL:{0}", (object)request.InnerRequest.RawUrl), (object)this);
        }

    }
}
