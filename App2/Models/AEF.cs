using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Xamarin.Forms;
using System.Linq.Expressions;

namespace App2.Models
{ // in android directory names seperated with /, not with \
   public class AEF 
    {
        static string DirectorySeperator = "/";
        static string Extension = ".afed";

        /*static byte[] Signature = { 0x54, 0x68, 0x69, 0x73, 0x20, 0x66, 0x69, 0x6c, 0x65, 0x20,
                                    0x69, 0x73, 0x20, 0x65, 0x6e, 0x63, 0x72, 0x79, 0x70, 0x74,
                                    0x65, 0x64, 0x20, 0x62, 0x79, 0x20, 0x48, 0x61, 0x73, 0x61,
                                    0x6e, 0x20, 0x41, 0x73, 0x6c, 0x61, 0x6e, 0x2e };
        */
        static byte[] Signature = Encoding.ASCII.GetBytes("This file has been encrypted by Hasan Aslan. Any modification will make it impossible to undo.");
        public static long CacheUsageAtTheMoment = 0;

        public static bool directoryCrawl = false;

        public string EncryptionMethod;
        public string OriginalPath { get; set; }  //storage/0/DCIM/1.jpg but this is original, not encrypted.
        public string ParentDirectory { get; set; }
        public string Name { get; set; } // file
        public string OriginalExtension { get; set; } // .jpg
        public string CurrentExtension { get; set; }
        public string FullName { get; set; } //file.jpg
        public string EncryptedFileName { get; set; } //afedASD+5R.afed
        public string OutputName { get; set; }  //storage/0/DCIM/1.jpg or //storage/0/DCIM/afedA^+Fas.afed
        public string ThumbPath { get; set; }
        public string FileSize { get; set; }
        public string AddedTime { get; set; }
        public DateTime ActualAddedTime { get; set; }

        public float ActualFileSize;
        public bool IsEncrypted { get; set; }
        public bool OnStream = false;
        public bool isSelected = false; // for listview
        
        public List<byte> OutputData;

        public AEF(string path)
        {
            this.OriginalPath = path;
            this.ParseName("init");
            this.AddedTime = DateTime.Now.ToString();
            this.FileSize = "Not calculated yet.";
            this.ThumbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "thumbs");
            this.ThumbPath += AEF.DirectorySeperator + AEF.GenerateRandomName(10) + ".jpg";
            this.ActualAddedTime = DateTime.Parse(this.AddedTime);
        }

        public AEF()
        {

        }


        public void ParseName(string operation)
        {
            switch (operation)
            {
                case "init":
                    this.FullName = Path.GetFileName(this.OriginalPath);
                    this.OriginalExtension = Path.GetExtension(this.OriginalPath);
                    this.CurrentExtension = this.OriginalExtension;
                    this.Name = Path.GetFileNameWithoutExtension(this.OriginalPath);
                    this.ParentDirectory = Path.GetDirectoryName(this.OriginalPath);
                    break;

                case "encrypt":
                    this.EncryptedFileName = AEF.GenerateRandomName(10) + AEF.Extension;
                    this.OutputName = this.ParentDirectory + AEF.DirectorySeperator + this.EncryptedFileName;
                    this.FileSize = this.CalculateFileSize();
                    this.ActualFileSize = this.ParseFileSize();
                    this.CurrentExtension = AEF.Extension;
                    break;

                case "decrypt":
                    this.CurrentExtension = this.OriginalExtension;
                    this.OutputName = this.OriginalPath; 
                    break;

                default:
                    throw new InvalidOperationException("Invalid ParseName parameter!");
            }

        }

        void SignFile()
        {
            this.OutputData = new List<byte>();
            foreach (byte b in AEF.Signature)
            {
                this.OutputData.Add(b);
            }
        }

        void CheckSignature(byte[] file)
        {
            if (this.CurrentExtension != AEF.Extension)
            {
                throw new Exception("File extension is not supported by this program. Due to performance issues, program won't attemp" +
                    " to decrypt this file unless you change the extension." + this.CurrentExtension + " - " + AEF.Extension);
            }
            for (int i = 0; i < AEF.Signature.Length; i++)
            {
                if (file[i] != AEF.Signature[i])
                {
                    throw new Exception("File is not signed with this program.");
                }
            }

        }

        public bool Encrypt()
        {
            this.EncryptionMethod = FileManipulation.ChooseMethod(this.OriginalExtension);
            byte[] fileBytes = File.ReadAllBytes(this.OriginalPath);
            this.SignFile();

            switch (this.EncryptionMethod)
            {
                case "Method 1":
                    return FileManipulation.EncryptMethod1(this, fileBytes);
                case "Method 2":
                    return FileManipulation.EncryptMethod2(this, fileBytes);
                case "Method 3":
                    return FileManipulation.EncryptMethod3(this, fileBytes);
                default:
                    return false;

            }

        }

        public bool Decrypt(bool toStream = false)
        {
            if (!this.OnStream) // if it's on stream, just use it.
            {
                byte[] fileBytes = File.ReadAllBytes(this.OutputName);
                this.OutputData = new List<byte>();
                this.CheckSignature(fileBytes);

                switch (this.EncryptionMethod)
                {
                    case "Method 1":
                        return FileManipulation.DecryptMethod1(this, fileBytes, toStream);
                    case "Method 2":
                        return FileManipulation.DecryptMethod2(this, fileBytes, toStream);
                    case "Method 3":
                        return FileManipulation.DecryptMethod3(this, fileBytes, toStream);
                    default:
                        return false;

                }

            }
            else
            {
                this.IsEncrypted = false;
                this.ParseName("decrypt");
            }
            return true;

        }

        public void Save()
        {
            File.WriteAllBytes(this.OutputName, this.OutputData.ToArray());
            if (this.IsEncrypted)
            { 
                File.Delete(this.OriginalPath);
                //clear memory
                this.OutputData.Clear();
            }
            else
            {
                File.Delete(this.ParentDirectory + AEF.DirectorySeperator + this.EncryptedFileName);
            }

        }

        public string GetAllProperties() //for admin
        { 
            string temp = "";
            foreach (var prop in this.GetType().GetProperties())
            {
                temp += prop.Name + " : " + prop.GetValue(this, null) + "\n";
            }
            return temp;
        }

        public string GetFileInfo()
        {
            return $"{this.FullName}\n\nPath: {this.OriginalPath}\n\nCurrent " +
                $"Path: {this.ParentDirectory + AEF.DirectorySeperator + this.EncryptedFileName}\n\nEncrypted at: {this.AddedTime}\n\nSize: {this.FileSize}";
            
        }

        public static string GenerateRandomName(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return "afed" + new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool GenerateThumbnail()
        {
            try
            {
               
                return true;
            }
            catch(Exception f)
            {
                Debug.WriteLine(f.Message);
                return false;
            }
        }
        
        public bool CopyToStream()
        {
            if (!this.OnStream)
            {
                try
                {
                    this.Decrypt(true); 
                    this.OnStream = true;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;

        }

        public bool IsPhoto()
        {
            string[] e = new string[] {".jpg",".jpeg", ".png", ".gif", "bmp" };
            foreach( string i in e)
            {
                if (this.OriginalExtension == i) return true;
            }
            return false;
        }

        public bool IsVideo()
        {
            string[] e = new string[] { ".mp4",".flv", ".3gp", ".webm", ".m4a", ".mp3" };
            foreach(string i in e)
            {
                if (i == this.OriginalExtension)
                {
                    return true;
                }
            }
            return false;
        }

        string CalculateFileSize()
        {
            long value = this.OutputData.Count;
            if (value < 1000)
            {
                return value.ToString() + " Bytes";
            }
            else if ( value > 999 && value < 999999)
            {
               
                return value.ToString().Substring(0, 3) + "," + value.ToString().Substring(3, 2) + " Kbs";
            }
            else if (value > 999999 && value < 1000000000)
            {
                if (value < 10000000)
                {
                    return value.ToString().Substring(0, 1) + "," + value.ToString().Substring(1, 2) + " Mbs";

                }
                else if (value < 100000000)
                {
                    return value.ToString().Substring(0, 2) + "," + value.ToString().Substring(2, 2) + " Mbs";

                }
                else
                {
                    return value.ToString().Substring(0, 3) + "," + value.ToString().Substring(3, 2) + " Mbs";

                }
            }
            else
            {
                return value.ToString().Substring(0, 1) + "," + value.ToString().Substring(1, 2) + " Gbs";

            }
           
        }

        public static int GetSignatureLength()
        {
            return AEF.Signature.Length;
        }

        public float ParseFileSize()
        {
            string[] values = this.FileSize.Split();
            float value = float.Parse(values[0]);
            switch(values[1])
            {
                case "Bytes":
                    return value;
                case "Kbs":
                    return value * 1000;
                case "Mbs":
                    return value * 1000000;
                default:
                    return value * 1000000000;
            }
        }
    }
}
