using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using App2.Business.Abstracts;

namespace App2.Business.Concretes
{
   public class ImageResizer
    {
        public ImageResizer() { }
        public Task<byte[]> ResizeImage(byte[] imageData)
        {
            return DependencyService.Get<IResizeImage>().ResizeImage(imageData, 150, 112);
        }

        public Task<byte[]> ResizeImage(string filepath)
        {
            byte[] imageData = System.IO.File.ReadAllBytes(filepath);
            return DependencyService.Get<IResizeImage>().ResizeImage(imageData, 150, 112);
        }

        public void SaveThumbnail(string filepath, byte[] data)
        {
            System.IO.File.WriteAllBytes(filepath, data);
        }
    }
}
