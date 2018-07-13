using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using PhotoCollage.Interfaces;
using UIKit;
using Photos;
using Xamarin.Forms;


[assembly: Dependency(typeof(PhotoCollage.iOS.Services.ImageStorageService))]
namespace PhotoCollage.iOS.Services
{
    public class ImageStorageService : IImageStorageService
    {
        public  Task<bool> SaveBitmap(byte[] bitmapData, string filename)
        {

            NSData data = NSData.FromArray(bitmapData);
            UIImage imasge = new UIImage(data);
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            imasge.SaveToPhotosAlbum((UIImage img, NSError error) =>
            {
                taskCompletionSource.SetResult(error == null);
            });

            return taskCompletionSource.Task;
            // First, check to see if we have initially asked the user for permission 
            // to access their photo album.
            //if (Photos.PHPhotoLibrary.AuthorizationStatus ==
            //    Photos.PHAuthorizationStatus.NotDetermined)
            //{
            //    var status =
            //        await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(
            //            Plugin.Permissions.Abstractions.Permission.Photos);
            //}
            //bool photoStored = false;
            //if (Photos.PHPhotoLibrary.AuthorizationStatus ==
            //    Photos.PHAuthorizationStatus.Authorized)
            //{
            //    // We have permission to access their photo album, 
            //    // so we can go ahead and save the image.
            //    var myImage = new UIImage(NSData.FromArray(bitmapData));
                
            //    myImage.SaveToPhotosAlbum((image, error) =>
            //    {
            //        if (error != null)
            //        {
            //            System.Diagnostics.Debug.WriteLine(error.ToString());                        
            //        }

            //        photoStored = true;
                    
            //    });
            //}
            //return photoStored;
        }
    }
}