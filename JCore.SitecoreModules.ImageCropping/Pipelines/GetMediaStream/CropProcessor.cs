using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCore.SitecoreModules.ImageCropping.Pipelines.GetMediaStream;
using JCore.SitecoreModules.ImageCropping.Resources.Media;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;

namespace JCore.SitecoreModules.ImageCropping.Pipelines.GetMediaStream
{
    /// <summary>
    /// CropProcessor class
    /// 
    /// </summary>
    public class CropProcessor
    {
        /// <summary>
        /// Runs the processor.
        /// 
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Process(GetMediaStreamPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            MediaStream outputStream = args.OutputStream;
            if (outputStream == null || args.Options.Thumbnail)
                return;
            if (!outputStream.AllowMemoryLoading)
            {
                Tracer.Error((object)"Could not crop image as it was larger than the maximum size allowed for memory processing. Media item: {0}", (object)outputStream.MediaItem.Path);
            }
            else
            {
                if (!args.MediaData.MimeType.StartsWith("image/", StringComparison.Ordinal))
                    return;
                string extension = args.MediaData.Extension;
                ImageFormat imageFormat = MediaManager.Config.GetImageFormat(extension, (ImageFormat)null);
                if (imageFormat == null)
                    return;
                CustomTransformationOptions transformationOptions = new CustomMediaOptions(args.Options).GetTransformationOptions();
                if (!transformationOptions.ContainsCropping())
                    return;
                this.ApplyBackgroundColor(args, imageFormat, transformationOptions);
                MediaStream mediaStream = outputStream;
                Stream stream = CustomMediaManager.Effects.TransformImageStream(mediaStream.Stream, transformationOptions, imageFormat);
                args.OutputStream = new MediaStream(stream, extension, mediaStream.MediaItem);
            }
        }

        /// <summary>
        /// Applies the color of the background.
        /// 
        /// </summary>
        /// <param name="args">The arguments.</param><param name="imageFormat">The image format.</param><param name="transformationOptions">The transformation options.</param>
        protected virtual void ApplyBackgroundColor(GetMediaStreamPipelineArgs args, ImageFormat imageFormat, CustomTransformationOptions transformationOptions)
        {
            Assert.ArgumentNotNull((object)args, "args");
            Assert.ArgumentNotNull((object)imageFormat, "imageFormat");
            Assert.ArgumentNotNull((object)transformationOptions, "transformationOptions");
            if (!transformationOptions.BackgroundColor.IsEmpty || imageFormat != ImageFormat.Bmp && imageFormat != ImageFormat.Gif && imageFormat != ImageFormat.Jpeg)
                return;
            transformationOptions.BackgroundColor = Settings.Media.DefaultImageBackgroundColor;
        }
    }
}
