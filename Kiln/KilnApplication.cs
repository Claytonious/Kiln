using System;
using System.IO;
using HtmlAgilityPack;
using System.Drawing;
using System.Web;
using System.Drawing.Imaging;

namespace Kiln
{
    class KilnApplication
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args == null || args.Length < 2 || args.Length > 3 || !File.Exists(args[0]))
                {
                    Log("USAGE: Kiln <input filename> <output filename> <max img width in pixels (optional)>");
                    Log("For input, specify the filename of a HTML file with <img> tags in it. Make sure that the relative or absolute paths to the images can be resolved from the working directory of Kiln.");
                }
                else
                {
                    int maxWidth = 4096;
                    if (args.Length > 2)
                    {
                        if (!Int32.TryParse(args[2], out maxWidth))
                            Log("Warning: unable to interpret [" + args[2] + "] as a number of pixels - ignoring max image width argument");
                    }
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(args[0]);
                    foreach(HtmlNode link in doc.DocumentNode.SelectNodes("//img[@src]"))
                    {
                        HtmlAttribute att = link.Attributes["src"];
                        Log("Processing " + att.Value + "...");
                        if (!File.Exists(att.Value))
                            Log("Warning: can't find referenced image file [" + att.Value + "] - skipping this img tag");
                        else
                        {
                            byte[] bitmapBuffer = null;
                            try
                            {
                                using (Bitmap bitmap = (Bitmap)Image.FromFile(att.Value))
                                {
                                    Bitmap downsampledBitmap = null;
                                    try
                                    {
                                        if (bitmap.Width > maxWidth)
                                        {
                                            int newWidth = maxWidth;
                                            double ratio = (double)bitmap.Height / (double)bitmap.Width;
                                            int newHeight = (int)(ratio * newWidth);
                                            Log("Downsampling from " + bitmap.Width + "," + bitmap.Height + " to " + newWidth + "," + newHeight);
                                            downsampledBitmap = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppPArgb);
                                            using (Graphics g = Graphics.FromImage(downsampledBitmap))
                                            {
                                                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                                g.DrawImage(bitmap, new Rectangle(0,0,downsampledBitmap.Width,downsampledBitmap.Height), new Rectangle(0,0,bitmap.Width,bitmap.Height), GraphicsUnit.Pixel);
                                            }
                                        }
                                        using (MemoryStream bitmapStream = new MemoryStream())
                                        {
                                            if (downsampledBitmap != null)
                                                downsampledBitmap.Save(bitmapStream, ImageFormat.Jpeg);
                                            else
                                                bitmap.Save(bitmapStream, ImageFormat.Jpeg);
                                            bitmapBuffer = bitmapStream.ToArray();
                                        }
                                    }
                                    finally
                                    {
                                        if (downsampledBitmap != null)
                                            downsampledBitmap.Dispose();
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                Log("Warning: can't load image file [" + att.Value + "] - skipping this img tag. The error was: " + ex);
                            }
                            if (bitmapBuffer != null)
                            {
                                string base64ImageData = HttpUtility.UrlEncode(Convert.ToBase64String(bitmapBuffer));
                                att.Value = "data:image/jpeg;base64," + base64ImageData;
                            }
                        }
                    }
                    doc.Save(args[1]);
                }
            }
            catch(Exception ex)
            {
                Log("Error: " + ex);
            }
        }

        public static void Log(string msg)
        {
            Console.Out.WriteLine(msg);
        }
    }
}
