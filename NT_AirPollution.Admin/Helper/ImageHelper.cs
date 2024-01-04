using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Web;

namespace NT_AirPollution.Admin.Helper
{
    public class ImageHelper
    {
        public static bool CompressImage(string srcPath, string distPath, int quality)
        {
            try
            {
                //原圖路徑
                using (Bitmap bmp = new Bitmap(srcPath))
                {
                    ImageCodecInfo codecInfo = GetEncoder(bmp.RawFormat); //圖片編解碼信息
                    Encoder encoder = Encoder.Quality;
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
                    encoderParameters.Param[0] = encoderParameter; //編碼器參數
                                                                   //壓縮圖路徑
                    ImageFormat format = bmp.RawFormat;

                    bmp.Save(distPath, codecInfo, encoderParameters); //保存壓縮圖
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat rawFormat)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == rawFormat.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}