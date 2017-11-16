using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Windows;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Integral.Handlers
{
    public class FotosHandler
    {
        private static readonly FotosHandler instancia = new FotosHandler();

        private FotosHandler() { }

        public static FotosHandler Instancia
        {
            get { return instancia; }
        }

        public ImageSource CreateImage(byte[] imageData, int decodePixelWidth, int decodePixelHeight)
        {
            if (imageData == null) return null;

            BitmapImage result = new BitmapImage();
            result.BeginInit();

            if (decodePixelWidth > 0)
            {
                result.DecodePixelWidth = decodePixelWidth;
            }

            if (decodePixelHeight > 0)
            {
                result.DecodePixelHeight = decodePixelHeight;
            }

            result.StreamSource = new MemoryStream(imageData);
            result.CreateOptions = BitmapCreateOptions.None;
            result.CacheOption = BitmapCacheOption.Default;
            result.EndInit();

            return result;
        }

        public ImageSource ImageSourceFromByteArray(byte[] imageData)
        {
            if (imageData == null) return null;

            BitmapImage result = new BitmapImage();
            result.BeginInit();
            result.StreamSource = new MemoryStream(imageData);
            result.CreateOptions = BitmapCreateOptions.None;
            result.CacheOption = BitmapCacheOption.Default;
            result.EndInit();
            return result;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public Bitmap GetSquareImage(Bitmap imgToSquare, System.Drawing.Size scale)
        {
            // Tile size
            var final_width  = Math.Min(scale.Width,imgToSquare.Width);
            var final_height = Math.Min(scale.Height,imgToSquare.Height);

            final_width = final_width = Math.Min(final_width, final_height);

            var x_start = (imgToSquare.Width -  final_width ) / 2;
            var y_start = (imgToSquare.Height - final_height) / 2;

            // Select source area
            var sourceData = imgToSquare.LockBits(new System.Drawing.Rectangle(x_start, y_start, final_width, final_height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                imgToSquare.PixelFormat);
            
            // Get bitmap for selected area
            var tile = new Bitmap(
                sourceData.Width,
                sourceData.Height,
                sourceData.Stride,
                sourceData.PixelFormat,
                sourceData.Scan0);

            // unlock area
            imgToSquare.UnlockBits(sourceData);

            return tile;
        }

        public Bitmap ResizeImage(Bitmap imgToResize, System.Drawing.Size size)
        {
            try
            {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((System.Drawing.Image)b))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }
                return b;
            }
            catch
            {
                Console.WriteLine("Bitmap could not be resized");
                return imgToResize;
            }
        }

        public byte[] GetEncodedImageData(ImageSource image, string preferredFormat)
        {

            byte[] result = null;
            BitmapEncoder encoder = null;

            switch (preferredFormat.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".tif":
                case ".tiff":
                    encoder = new TiffBitmapEncoder();
                    break;
                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;
                case ".wmp":
                    encoder = new WmpBitmapEncoder();
                    break;
            }

            if (image is BitmapSource)
            {

                MemoryStream stream = new MemoryStream();
                encoder.Frames.Add(BitmapFrame.Create(image as BitmapSource));
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                result = new byte[stream.Length];
                BinaryReader br = new BinaryReader(stream);
                br.Read(result, 0, (int)stream.Length);
                br.Close();
                stream.Close();
            }

            return result;
        }

        public byte[] LoadImageData(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] imageBytes = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            return imageBytes;
        }

        public void SaveImageData(byte[] imageData, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(imageData);
            bw.Close();
            fs.Close();
        }

        public void DeleteImageData(long socioId)
        {
            string directory = Directory.GetCurrentDirectory();
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("es-ES");
            string id_s = ((long)socioId).ToString(cultura);
            string img_path = System.IO.Path.Combine(directory, @"imagenes\" + id_s + "_256.dat");

            if (File.Exists(img_path))
            {
                File.Delete(img_path);
            }
        }
    }
}
