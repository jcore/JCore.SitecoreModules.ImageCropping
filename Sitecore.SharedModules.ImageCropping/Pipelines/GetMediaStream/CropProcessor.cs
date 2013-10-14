using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Collections;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.SharedModules.ImageCropping.Resources.Media;

namespace Sitecore.SharedModules.ImageCropping.Pipelines.GetMediaStream
{
    /// <summary>
    /// Pipeline processor that is responsible for image cropping.
    /// </summary>
    public class CropProcessor
    {

        /// <summary>
        /// Starts the Processor. Requires "Media.Video_filetypes" setting in web.config. Crops specified image based on the passed crop region.
        /// </summary>
        /// <param name="args">Sitecore.Resources.Media.GetMediaStreamPipelineArgs.</param>
        public void Process(Sitecore.Resources.Media.GetMediaStreamPipelineArgs args)
        {

            string _rgn = "";
            int _croppedWidth = 0;
            int _croppedHeight = 0;
            string extension = args.MediaData.Extension;

            if (Sitecore.Configuration.Settings.GetSetting("Media.Video_filetypes").IndexOf(extension) > -1)
            {
                //args.Options.Thumbnail = true;
                return;
            }

            if (Sitecore.Configuration.Settings.GetSetting("Media.Doc_filetypes").IndexOf(extension) > -1)
            {
                return;
            }


            StringDictionary sDec = args.Options.CustomOptions;

            MediaStream outputStream = args.OutputStream;

            IEnumerator<string> enumerator = sDec.Keys.GetEnumerator();
            IEnumerator<string> enumeratorValues = sDec.Values.GetEnumerator();

            while (enumerator.MoveNext())
            {
                enumeratorValues.MoveNext();

                string current = enumerator.Current;
                string currentValue = enumeratorValues.Current;

                if (current == "rgn")
                {
                    _rgn = currentValue;
                }
                else if (current == "cw")
                {
                    _croppedWidth = Convert.ToInt32(currentValue);
                }
                else if (current == "ch")
                {
                    _croppedHeight = Convert.ToInt32(currentValue);
                }
            }

            if (_rgn != "0,0,0,0" && _rgn != "" && !args.Options.Thumbnail)
            {
                if (outputStream != null)
                {
                    CropOptions cropOptions = new CropOptions();
                    TransformationOptions transformationOptions = args.Options.GetTransformationOptions();
                    cropOptions.AllowStretch = transformationOptions.AllowStretch;
                    cropOptions.BackgroundColor = transformationOptions.BackgroundColor;
                    cropOptions.MaxSize = transformationOptions.MaxSize;
                    cropOptions.Scale = transformationOptions.Scale;
                    cropOptions.Size = transformationOptions.Size;
                    cropOptions.Region = _rgn;
                    cropOptions.CroppedHeight = _croppedHeight;
                    cropOptions.CroppedWidth = _croppedWidth;

                    ImageFormat imageFormat = MediaManager.Config.GetImageFormat(extension);
                    Assert.IsNotNull(imageFormat, typeof(ImageFormat), "Extension: '{0}'.", new object[] { extension });
                    Stream stream2 = ImageEffectsCrop.CropImageStream(outputStream.Stream, cropOptions, imageFormat);
                    if (stream2 != null)
                    {
                        args.OutputStream = new MediaStream(stream2, extension, outputStream.MediaItem);
                    }
                }
                else
                {
                    Log.Error("--------  CropProcessor : outputStream is null", this);
                }
            }

            return;
        }

    }
}
