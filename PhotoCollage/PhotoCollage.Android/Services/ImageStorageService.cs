using System;
using System.Threading.Tasks;
using PhotoCollage.Interfaces;
using Android.Media;
using Android.Widget;
using Java.IO;
using Xamarin.Forms;
using Environment = Android.OS.Environment;

[assembly: Dependency(typeof(PhotoCollage.Droid.Services.ImageStorageService))]
namespace PhotoCollage.Droid.Services
{
    class ImageStorageService : IImageStorageService
    {
        public async Task<bool> SaveBitmap(byte[] bitmapData, string filename)
        {
            try
            {
                File picturesDirectory = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures);
                File spinPaintDirectory = new File(picturesDirectory, "PhotoCollage");
                spinPaintDirectory.Mkdirs();

                using (File bitmapFile = new File(spinPaintDirectory, filename))
                {
                    bitmapFile.CreateNewFile();

                    using (FileOutputStream outputStream = new FileOutputStream(bitmapFile))
                    {
                        await outputStream.WriteAsync(bitmapData);
                    }
                    MediaScannerConnection.ScanFile(Android.App.Application.Context, new string[] { bitmapFile.Path }, new string[] { "image/png", "image/jpeg" }, null);
                    return true;
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(Android.App.Application.Context, ex.Message,ToastLength.Long).Show();
                return false;
            }
        }
    }
}