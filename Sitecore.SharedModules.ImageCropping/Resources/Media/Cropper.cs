using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Diagnostics;

namespace Sitecore.SharedModules.ImageCropping.Resources.Media
{
    /// <summary>
    /// Performs cropping operations in the passed bitmap image based on the crop options.
    /// </summary>
    public class Cropper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cropper"/> class.
        /// </summary>
        public Cropper() { }

        /// <summary>
        /// Crops the specified original image. Determines image format and calls appropriate methods for jpeg and gif image formats.
        /// </summary>
        /// <param name="originalImage">The original image.</param>
        /// <param name="options">The options.</param>
        /// <param name="outputFormat">The output format.</param>
        /// <returns>Cropped bitmap</returns>
        public Bitmap Crop(Bitmap originalImage, CustomTransformationOptions options, ImageFormat outputFormat)
        {
            if (outputFormat == ImageFormat.Gif)
            {
                return this.CropGif(originalImage, options);
            }
            return this.CropAny(originalImage, options);
        }

        /// <summary>
        /// Crops the GIF.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="options">The options.</param>
        /// <returns>Cropped bitmap for GIF image</returns>
        private Bitmap CropGif(Bitmap image, CustomTransformationOptions options)
        {
            int _x1 = 0;
            int _y1 = 0;
            int _x2 = 0;
            int _y2 = 0;

            string[] region = options.CropRegion;
            if (region.Length >= 4)
            {
                _x1 = int.TryParse(region[0], out _x1) ? _x1 : 0;
                _y1 = int.TryParse(region[1], out _y1) ? _y1 : 0;
                _x2 = int.TryParse(region[2], out _x2) ? _x2 : 0;
                _y2 = int.TryParse(region[3], out _y2) ? _y2 : 0;

                if (_x2 > image.Width) _x2 = image.Width;
                if (_y2 > image.Height) _y2 = image.Height;

                if (_x1 < 0) _x1 = 0;
                if (_y1 < 0) _y1 = 0;

            }

            int newWidth = _x2 - _x1;
            int newHeight = _y2 - _y1;
            Size size = new Size(newWidth, newHeight);

            int colorIndex = this.GetColorIndex(options.BackgroundColor, image);
            if (colorIndex < 0)
            {
                colorIndex = this.ReplaceLeastUsedColor(image, options.BackgroundColor);
            }
            Bitmap frame = this.CreateGifFrame(size, (byte)colorIndex, image);
            return this.OverlayGif(image, frame, options.CropRegion);
        }
        /// <summary>
        /// Creates a new frame for the GIF.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="image">The image.</param>
        /// <returns>Frame for new GIF image.</returns>
        private Bitmap CreateGifFrame(Size size, byte backgroundColor, Bitmap image)
        {
            Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format8bppIndexed);
            ColorPalette palette = image.Palette;
            ColorPalette palette2 = bitmap.Palette;
            for (int i = 0; i < palette.Entries.Length; i++)
            {
                palette2.Entries[i] = palette.Entries[i];
            }
            bitmap.Palette = palette2;

            this.ClearGif(bitmap, backgroundColor);
            return bitmap;
        }
        /// <summary>
        /// Overlays the GIF.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="rgn">The RGN.</param>
        /// <returns></returns>
        private unsafe Bitmap OverlayGif(Bitmap image, Bitmap frame, string[] rgn)
        {
            ColorPalette palette = image.Palette;
            int num = 0;
            int num2 = 0;
            int _x1 = 0;
            int _y1 = 0;
            int _x2 = 0;
            int _y2 = 0;

            string[] region = rgn;

            if (region.Length >= 4)
            {
                _x1 = int.TryParse(region[0], out _x1) ? _x1 : 0;
                _y1 = int.TryParse(region[1], out _y1) ? _y1 : 0;
                _x2 = int.TryParse(region[2], out _x2) ? _x2 : 0;
                _y2 = int.TryParse(region[3], out _y2) ? _y2 : 0;

                if (_x2 > image.Width) _x2 = image.Width;
                if (_y2 > image.Height) _y2 = image.Height;

                if (_x1 < 0) _x1 = 0;
                if (_y1 < 0) _y1 = 0;

            }

            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                BitmapData bitmapdata = frame.LockBits(new Rectangle(0, 0, frame.Width, frame.Height), ImageLockMode.WriteOnly, frame.PixelFormat);
                try
                {
                    int stride = bitmapdata.Stride;
                    byte* numPtr = (byte*)bitmapdata.Scan0;
                    numPtr += num + (num2 * stride);
                    int num4 = stride - frame.Width;
                    for (int i = _y1; i < _y2; i++)
                    {
                        for (int j = _x1; j < _x2; j++)
                        {
                            byte index = this.GetColorIndex(j, i, data);
                            if (palette.Entries[index].A > 0)
                            {
                                numPtr[0] = index;
                            }
                            numPtr++;
                        }
                        numPtr += num4;
                    }
                    return frame;
                }
                finally
                {
                    frame.UnlockBits(bitmapdata);
                }
            }
            finally
            {
                image.UnlockBits(data);
            }
        }
        /// <summary>
        /// Gets the index of the color.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="data">The bitmap data.</param>
        /// <returns></returns>
        private unsafe byte GetColorIndex(int x, int y, BitmapData data)
        {
            int stride = data.Stride;
            byte* numPtr = (byte*)data.Scan0;
            numPtr += x + (y * stride);
            return numPtr[0];
        }
        /// <summary>
        /// Clears the GIF.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        private unsafe void ClearGif(Bitmap image, byte backgroundColor)
        {
            BitmapData bitmapdata = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, image.PixelFormat);
            try
            {
                int stride = bitmapdata.Stride;
                byte* numPtr = (byte*)bitmapdata.Scan0;
                int num2 = stride - image.Width;
                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        numPtr[0] = backgroundColor;
                        numPtr++;
                    }
                    numPtr += num2;
                }
            }
            finally
            {
                image.UnlockBits(bitmapdata);
            }
        }

        /// <summary>
        /// Replaces the least used color.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns></returns>
        private byte ReplaceLeastUsedColor(Bitmap image, Color replacement)
        {
            byte leastUsedColorIndex = this.GetLeastUsedColorIndex(image);
            ColorPalette palette = image.Palette;
            if (leastUsedColorIndex < palette.Entries.Length) palette.Entries[leastUsedColorIndex] = replacement;
            image.Palette = palette;
            return leastUsedColorIndex;
        }
        /// <summary>
        /// Gets the index of the least used color.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Color index in byte format.</returns>
        private byte GetLeastUsedColorIndex(Bitmap image)
        {
            int[] paletteUsage = this.GetPaletteUsage(image);
            int num = 0;
            int num2 = 0x7fffffff;

            for (int i = 0; i < paletteUsage.Length; i++)
            {
                if (paletteUsage[i] == 0)
                {
                    return (byte)i;
                }
                if (paletteUsage[i] < num2)
                {
                    num2 = paletteUsage[i];
                    num = i;
                }
            }

            return (byte)num;
        }
        /// <summary>
        /// Gets the palette usage.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Array of int.</returns>
        private unsafe int[] GetPaletteUsage(Bitmap image)
        {
            int[] numArray = new int[0x100];
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = 0;
            }

            BitmapData bitmapdata = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                int stride = bitmapdata.Stride;
                byte* numPtr = (byte*)bitmapdata.Scan0;
                int num3 = stride - image.Width;
                for (int j = 0; j < image.Height; j++)
                {
                    for (int k = 0; k < image.Width; k++)
                    {
                        byte index = numPtr[0];
                        numArray[index]++;
                        numPtr++;
                    }
                    numPtr += num3;
                }
            }
            finally
            {
                image.UnlockBits(bitmapdata);
            }
            return numArray;
        }

        /// <summary>
        /// Crops image if it's format is not GIF.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="options">The options.</param>
        /// <returns>Cropped bitmap.</returns>
        private Bitmap CropAny(Image image, CustomTransformationOptions options)
        {
            try
            {
                int _x1 = 0;
                int _y1 = 0;
                int _x2 = 0;
                int _y2 = 0;

                string[] region = options.CropRegion;

                if (region.Length >= 4)
                {
                    _x1 = int.TryParse(region[0], out _x1) ? _x1 : 0;
                    _y1 = int.TryParse(region[1], out _y1) ? _y1 : 0;
                    _x2 = int.TryParse(region[2], out _x2) ? _x2 : 0;
                    _y2 = int.TryParse(region[3], out _y2) ? _y2 : 0;

                    if (_x2 > image.Width) _x2 = image.Width;
                    if (_y2 > image.Height) _y2 = image.Height;

                    if (_x1 < 0) _x1 = 0;
                    if (_y1 < 0) _y1 = 0;

                }

                int newWidth = _x2 - _x1;
                int newHeight = _y2 - _y1;

                //cropping  

                var bmpImage = new Bitmap(image);
                var rectangle = new Rectangle(_x1, _y1, newWidth, newHeight);
                var newImage = bmpImage.Clone(rectangle, bmpImage.PixelFormat);

                return newImage;

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
                return new Bitmap(image);

            }
        }
        /// <summary>
        /// Gets the index of the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="image">The image.</param>
        /// <returns>Color index as int.</returns>
        private int GetColorIndex(Color color, Image image)
        {
            bool flag = color == Color.Transparent;
            ColorPalette palette = image.Palette;
            for (int i = 0; i < palette.Entries.Length; i++)
            {
                Color color2 = palette.Entries[i];
                if (flag && (color2.A == 0))
                {
                    return i;
                }
                if (((color2.R == color.R) && (color2.G == color.G)) && (color2.B == color.B))
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
