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

namespace Sitecore.SharedModules.ImageCropping.Resources.Media
{
    class ImageCropEffects : Sitecore.Resources.Media.ImageEffectsResize
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageEffectsCrop"/> class.
        /// </summary>
        public ImageCropEffects()
        {

        }

        /// <summary>
        /// Crops the image stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="options">The options.</param>
        /// <param name="outputFormat">The output format.</param>
        /// <returns></returns>
        public static Stream CropImageStream(Stream inputStream, CropOptions options, ImageFormat outputFormat)
        {

            Assert.ArgumentNotNull(inputStream, "inputStream");
            Assert.ArgumentNotNull(options, "options");
            Assert.ArgumentNotNull(outputFormat, "outputFormat");

            System.Drawing.Bitmap newImage;

            if (inputStream.Length <= Settings.Media.MaxSizeInMemory)
            {
                if (options.Region != "0,0,0,0")
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

        /// <summary>
        /// Gets the resize options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        private ResizeOptions GetResizeOptions(CropOptions options)
        {
            ResizeOptions options2 = new ResizeOptions();
            options2.AllowStretch = options.AllowStretch;
            options2.BackgroundColor = options.BackgroundColor;
            options2.MaxSize = options.MaxSize;
            options2.Scale = options.Scale;
            options2.Size = options.Size;
            return options2;
        }
    }
}
