using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.ImageLib;

namespace Sitecore.SharedModules.ImageCropping.ImageLib
{
    public class Resizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Resizer"/> class.
        /// </summary>
        public Resizer()
        {
        }

        /// <summary>
        /// Resizes the specified bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="imageSize">Size of the image.</param>
        /// <param name="frameSize">Size of the frame.</param>
        /// <param name="frameColor">Color of the frame.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public Bitmap Resize(Bitmap bitmap, Size imageSize, Size frameSize, Color frameColor, ImageFormat format)
        {
            if (format == ImageFormat.Gif)
            {
                return this.ResizeGif(bitmap, imageSize, frameSize, frameColor);
            }
            return this.ResizeAny(bitmap, imageSize, frameSize, frameColor);
        }

        /// <summary>
        /// Resizes the specified bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="options">The options.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public Bitmap Resize(Bitmap bitmap, ResizeOptions options, ImageFormat format)
        {
            Size frameSize = this.GetFrameSize(bitmap, options);
            Size imageSize = this.GetImageSize(bitmap, frameSize, options);
            return this.Resize(bitmap, imageSize, frameSize, options.BackgroundColor, format);
        }


        /// <summary>
        /// Resizes any.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="imageSize">Size of the image.</param>
        /// <param name="frameSize">Size of the frame.</param>
        /// <param name="frameColor">Color of the frame.</param>
        /// <returns></returns>
        private Bitmap ResizeAny(Bitmap image, Size imageSize, Size frameSize, Color frameColor)
        {
            if (frameSize == image.Size)
            {
                return image;
            }
            int x = (frameSize.Width - imageSize.Width) / 2;
            int y = (frameSize.Height - imageSize.Height) / 2;
            Rectangle rect = new Rectangle(x, y, imageSize.Width, imageSize.Height);
            Bitmap bitmap = new Bitmap(image, frameSize);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.Clear(frameColor);
                graphics.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            }
            System.Drawing.Imaging.ImageCodecInfo[] arrayICI = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            System.Drawing.Imaging.ImageCodecInfo jpegICI = null;
            for (int i = 0; i < arrayICI.Length; i++)
            {
                if (arrayICI[i].FormatDescription.Equals("JPEG"))
                {
                    jpegICI = arrayICI[i];
                }
            }
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            System.Drawing.Imaging.EncoderParameters myEncoderParameters = new EncoderParameters(1);
            System.Drawing.Imaging.EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, jpegICI, myEncoderParameters);
            Bitmap bitmap_tmp = new Bitmap(memoryStream);
            return bitmap_tmp;
        }

        /// <summary>
        /// Resizes the GIF.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="imageSize">Size of the image.</param>
        /// <param name="frameSize">Size of the frame.</param>
        /// <param name="frameColor">Color of the frame.</param>
        /// <returns></returns>
        private Bitmap ResizeGif(Bitmap image, Size imageSize, Size frameSize, Color frameColor)
        {
            image = this.ResizeGif(image, imageSize);
            if (imageSize == frameSize)
            {
                return image;
            }

            int colorIndex = this.GetColorIndex(frameColor, image);
            if (colorIndex < 0)
            {
                colorIndex = this.ReplaceLeastUsedColor(image, frameColor);
            }
            Bitmap frame = this.CreateGifFrame(frameSize, (byte)colorIndex, image);
            return this.OverlayGif(image, frame);
        }


        /// <summary>
        /// Resizes the GIF.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        private Bitmap ResizeGif(Bitmap image, Size size)
        {
            if (image.Size == size)
            {
                return image;
            }
            ThumbMaker maker = new ThumbMaker(image);
            return maker.ResizeToGif(size);
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
        /// Creates the GIF frame.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="image">The image.</param>
        /// <returns></returns>
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
        /// Gets the index of the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        private int GetColorIndex(Color color, Bitmap image)
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



        /// <summary>
        /// Gets the index of the color.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private unsafe byte GetColorIndex(int x, int y, BitmapData data)
        {
            int stride = data.Stride;
            byte* numPtr = (byte*)data.Scan0;
            numPtr += x + (y * stride);
            return numPtr[0];
        }


        /// <summary>
        /// Gets the size of the frame.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        private Size GetFrameSize(Bitmap image, ResizeOptions options)
        {
            Size preferredSize = this.GetPreferredSize(options, image);
            return LimitToMaxSize(options, preferredSize);
        }


        /// <summary>
        /// Gets the size of the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="frameSize">Size of the frame.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        private Size GetImageSize(Bitmap image, Size frameSize, ResizeOptions options)
        {
            float scale = GetScale(options, image, frameSize);
            int width = this.Scale(image.Width, scale);
            return new Size(width, this.Scale(image.Height, scale));
        }

        /// <summary>
        /// Gets the index of the least used color.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// Gets the size of the preferred.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        private Size GetPreferredSize(ResizeOptions options, Image image)
        {
            if (options.Scale > 0f)
            {
                return new Size(this.Scale(image.Width, options.Scale), this.Scale(image.Height, options.Scale));
            }
            if (options.Size.IsEmpty || (options.Size == image.Size))
            {
                return new Size(image.Size.Width, image.Size.Height);
            }
            if (options.Size.Width == 0)
            {
                float scale = ((float)options.Size.Height) / ((float)image.Height);
                return new Size(this.Scale(image.Width, scale), options.Size.Height);
            }
            if (options.Size.Height == 0)
            {
                float num2 = ((float)options.Size.Width) / ((float)image.Width);
                return new Size(options.Size.Width, this.Scale(image.Height, num2));
            }
            return new Size(options.Size.Width, options.Size.Height);
        }


        /// <summary>
        /// Gets the scale.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="originalImage">The original image.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        private static float GetScale(ResizeOptions options, Image originalImage, Size size)
        {
            float num = ((float)size.Width) / ((float)originalImage.Width);
            float num2 = ((float)size.Height) / ((float)originalImage.Height);
            float num3 = Math.Min(num, num2);
            if (!options.AllowStretch && (num3 > 1f))
            {
                num3 = 1f;
            }
            return num3;
        }

        /// <summary>
        /// Limits the size of to max.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        private static Size LimitToMaxSize(ResizeOptions options, Size size)
        {
            if (!options.MaxSize.IsEmpty)
            {
                if ((options.MaxSize.Width > 0) && (size.Width > options.MaxSize.Width))
                {
                    if (options.Size.Height == 0)
                    {
                        size.Height = (int)Math.Round((double)((((float)options.MaxSize.Width) / ((float)size.Width)) * size.Height));
                    }
                    size.Width = options.MaxSize.Width;
                }
                if ((options.MaxSize.Height <= 0) || (size.Height <= options.MaxSize.Height))
                {
                    return size;
                }
                if (options.Size.Width == 0)
                {
                    size.Width = (int)Math.Round((double)((((float)options.MaxSize.Height) / ((float)size.Height)) * size.Width));
                }
                size.Height = options.MaxSize.Height;
            }
            return size;
        }

        /// <summary>
        /// Overlays the GIF.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="frame">The frame.</param>
        /// <returns></returns>
        private unsafe Bitmap OverlayGif(Bitmap image, Bitmap frame)
        {
            ColorPalette palette = image.Palette;
            int num = (frame.Width - image.Width) / 2;
            int num2 = (frame.Height - image.Height) / 2;
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            try
            {
                BitmapData bitmapdata = frame.LockBits(new Rectangle(0, 0, frame.Width, frame.Height), ImageLockMode.WriteOnly, frame.PixelFormat);
                try
                {
                    int stride = bitmapdata.Stride;
                    byte* numPtr = (byte*)bitmapdata.Scan0;
                    numPtr += num + (num2 * stride);
                    int num4 = stride - image.Width;
                    for (int i = 0; i < image.Height; i++)
                    {
                        for (int j = 0; j < image.Width; j++)
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
            return frame;
        }



        /// <summary>
        /// Replaces the color of the least used.
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
        /// Scales the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        private int Scale(int value, float scale)
        {
            return (int)Math.Round((double)(value * scale));
        }



    }
}
