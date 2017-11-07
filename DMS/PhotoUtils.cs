/******************************************
-- Author       : Coder.Yan
-- Create date  : 2011-11-24 13:25:24
-- CopyRight    : HZST (C) 2011
-- Description  : 图像转换类
 ******************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DMS
{
    public class PhotoUtils
    {
        /// <summary>
        /// 将图片转换为Base64格式字符串
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string ToBase64String(Image img)
        {
            return ToBase64String(img, ImageFormat.Png);
        }

        public static string ToBase64String(Image img, ImageFormat format)
        {
            if (img != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, format);
                    byte[] buffer = ms.ToArray();
                    return Convert.ToBase64String(buffer);
                }
            }
            return string.Empty;
        }

        public static string ToBase64HtmlString(Image img)
        {
            return ToBase64HtmlString(img, ImageFormat.Png);
        }

        public static string ToBase64HtmlString(Image img, ImageFormat format)
        {
            string type = "jpeg";
            if (format.Guid == ImageFormat.Png.Guid)
            {
                type = "png";
            }
            else if (format.Guid == ImageFormat.Gif.Guid)
            {
                type = "gif";
            }
            return string.Format("data:image/{0};base64,", type) + ToBase64String(img, format);
        }

        public static Image FromBase64String(string base64Str)
        {
            Bitmap bitmap = null;
            Image img = null;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = Convert.FromBase64String(base64Str);
                ms.Write(buffer, 0, buffer.Length);
                try
                {
                    img = Image.FromStream(ms);
                    if (img != null)
                    {
                        bitmap = new Bitmap(img.Width, img.Height);
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.DrawImage(img, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                        }
                    }
                }
                catch { }
            }
            return bitmap;
        }

        public static Image FromBase64HtmlString(string str)
        {
            string[] strs = str.Split(',');
            if (strs.Length > 0)
            {
                return FromBase64String(strs[strs.Length - 1]);
            }
            else
            {
                return FromBase64String(str);
            }
        }
    }
}