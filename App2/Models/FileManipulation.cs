using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace App2.Models
{
    public class FileManipulation
    {
        static string[] EncryptionMethods = new string[] { "Method 1", "Method 2", "Method 3" }; //
        static Random r = new Random();

        public static string ChooseMethod(string extension)
        {
            string method;
            while(true)
            {
                method = FileManipulation.EncryptionMethods[FileManipulation.r.Next(0, FileManipulation.EncryptionMethods.Length)];
                if(extension == ".txt" && method == "Method 1")
                {
                    continue;
                }
                return method;
            }
          
        }
             

        public static bool EncryptMethod1(AEF file ,byte[] fileBytes)
        {
            try
            {
                int start = fileBytes.Length - 1;
                for (int i = start; i >= 0; i--)
                {
                    file.OutputData.Add(fileBytes[i]);
                }
                file.IsEncrypted = true;
                file.ParseName("encrypt");
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        public static bool DecryptMethod1(AEF file, byte[] fileBytes, bool toStream = false)
        {

            try
            {
                int target = AEF.GetSignatureLength() - 1;
                int start = fileBytes.Length - 1;
                for (int i = start; i > target; i--)
                {
                    file.OutputData.Add(fileBytes[i]);
                }
                if (!toStream)
                {
                    file.IsEncrypted = false;
                    file.ParseName("decrypt");
                }
              
                return true;
            }
            catch(Exception)
            {
                return false;
            }
            
          
        }

        public static bool EncryptMethod2(AEF file, byte[] fileBytes)
        {
            try
            {
                int x = 0;
                int target = fileBytes.Length;
                for (int i = 0; i < target; i++)
                {
                    x = fileBytes[i] + 1;
                    if (x == 256) x = 0;
                    file.OutputData.Add((byte)x);
                }
                file.IsEncrypted = true;
                file.ParseName("encrypt");
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        public static bool DecryptMethod2(AEF file, byte[] fileBytes, bool toStream )
        {
            try
            {
                int start = AEF.GetSignatureLength();
                int target = fileBytes.Length;
                int x = 0;

                for (int i = start; i < target; i++)
                {
                    x = fileBytes[i] - 1;
                    if (x <= -1) x = 255;
                    file.OutputData.Add((byte)x);
                }
                if (!toStream)
                {
                    file.IsEncrypted = false;
                    file.ParseName("decrypt");
                }
                
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public static bool EncryptMethod3(AEF file, byte[] fileBytes)
        {
            try
            {
                int x = 0;
                int target = fileBytes.Length;
                for (int i = 0; i < target; i++)
                {
                    x = fileBytes[i] - 1;
                    if (x == -1) x = 255;
                    file.OutputData.Add((byte)x);
                }
                file.IsEncrypted = true;
                file.ParseName("encrypt");
                return true;
            }

            catch (Exception)
            {
                return false;
            }

        }

        public static bool DecryptMethod3(AEF file, byte[] fileBytes, bool toStream)
        {
            try
            {
                int start = AEF.GetSignatureLength();
                int target = fileBytes.Length;
                int x = 0;

                for (int i = start; i < target; i++)
                {
                    x = fileBytes[i] + 1;
                    if (x == 256) x = 0;
                    file.OutputData.Add((byte)x);
                }
                if (!toStream)
                {
                    file.IsEncrypted = false;
                    file.ParseName("decrypt");
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
