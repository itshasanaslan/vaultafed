using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace App2.Business.Concretes
{
    public class ThumbPathFinder
    {
        public static string Get()
        {
            string path = string.Empty;
            if (Device.RuntimePlatform == Device.iOS)
            {

                var sqliteFileName = ".thumbs";
                string documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var libraryPath = Path.Combine(documentPath, "..", "Library");
                path = Path.Combine(libraryPath, sqliteFileName);
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                var sqliteFileName = ".thumbs";
                string documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                path = Path.Combine(documentPath, sqliteFileName);
            }
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }
}
