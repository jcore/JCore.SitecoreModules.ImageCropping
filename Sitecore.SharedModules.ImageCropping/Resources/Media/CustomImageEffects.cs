using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.ImageLib;
using Sitecore.IO;
using Sitecore.Resources.Media;

namespace Sitecore.SharedModules.ImageCropping.Resources.Media
{
    /// <summary>
    /// CustomImageEffects class
    /// 
    /// </summary>
    public class CustomImageEffects : ImageEffects
    {
        /// <summary>
        /// Transforms an image stream.
        /// 
        /// </summary>
        /// <param name="inputStream">The stream containing the media data.</param><param name="options">The image options.</param><param name="outputFormat">The image format of the resulting thumbnail image.</param>
        /// <returns/>
        public virtual Stream TransformImageStream(Stream inputStream, CustomTransformationOptions options, ImageFormat outputFormat)
        {
            Assert.ArgumentNotNull((object)inputStream, "inputStream");
            Assert.ArgumentNotNull((object)options, "options");
            Assert.ArgumentNotNull((object)outputFormat, "outputFormat");
            return this.CropImageStream(inputStream, options, outputFormat);
        }

        /// <summary>
        /// Resizes an image represented by a stream.
        /// 
        /// </summary>
        /// <param name="inputStream">The input stream.</param><param name="options">The options.</param><param name="outputFormat">The output format.</param>
        /// <returns>
        /// The image stream.
        /// </returns>
        public Stream CropImageStream(Stream inputStream, CustomTransformationOptions options, ImageFormat outputFormat)
        {
            Assert.ArgumentNotNull((object)inputStream, "inputStream");
            Assert.ArgumentNotNull((object)options, "options");
            Assert.ArgumentNotNull((object)outputFormat, "outputFormat");
            Bitmap newImage;

            if (inputStream.Length <= Settings.Media.MaxSizeInMemory)
            {
                if (options.CropRegion != null && options.CropRegion.Count()  == 4)
                {
                    MemoryStream stream = new MemoryStream();
                    newImage = new Cropper().Crop(new Bitmap(inputStream), options, outputFormat);
                    newImage.Save(stream, outputFormat);

                    stream.Seek(0L, SeekOrigin.Begin);

                    newImage.Dispose();

                    return stream;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Tracer.Error("Could not crop image stream as it was larger than the maximum size allowed for memory processing.");
                return null;
            }
        }

        private ImageCodecInfo FindEncoderInfo(ImageFormat outputFormat)
        {
            throw new NotImplementedException();
        }

        private Stream CropLegacy(Stream inputStream, CustomTransformationOptions options, ImageFormat outputFormat)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the crop options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        private CustomTransformationOptions GetCropOptions(CustomTransformationOptions options)
        {
            return new CustomTransformationOptions()
            {
                AllowStretch = options.AllowStretch,
                BackgroundColor = options.BackgroundColor,
                IgnoreAspectRatio = options.IgnoreAspectRatio,
                MaxSize = options.MaxSize,
                Scale = options.Scale,
                Size = options.Size,
                PreserveResolution = options.PreserveResolution,
                CompositingMode = options.CompositingMode,
                InterpolationMode = options.InterpolationMode,
                PixelOffsetMode = options.PixelOffsetMode,
                CropRegion = options.CropRegion
            };
        }
    }
}
