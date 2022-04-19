using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App2.Business.Abstracts;
using Android.Graphics;
using System.Threading.Tasks;
using System.IO;
using App2.Droid.Business;

[assembly: Xamarin.Forms.Dependency(typeof(ImageResizerDroid))]
namespace App2.Droid.Business
{
   public class ImageResizerDroid : IResizeImage
    {
        public ImageResizerDroid() { }

        public Task<byte[]> ResizeImage(byte[] imageData, float width, float height)
        {
            
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return Task.Run(
                    () => ms.ToArray()
                    );


            }
        }
    }
}