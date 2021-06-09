using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Utility
{
    public class FileService
    {
       
        public static bool FileFormatChecker(byte[] fileBytes)
        {
            var firstBytes4 = fileBytes.Take(4).ToArray();
            var firstBytes3 = firstBytes4.Take(3).ToArray();
            if (Enumerable.SequenceEqual(firstBytes3, Constants.tiff) || Enumerable.SequenceEqual(firstBytes3, Constants.tiff2)) return true;
            else if (Enumerable.SequenceEqual(firstBytes4, Constants.png) || (Enumerable.SequenceEqual(firstBytes4, Constants.jpg)) || Enumerable.SequenceEqual(firstBytes4, Constants.jpeg) || Enumerable.SequenceEqual(firstBytes4, Constants.jpeg2)) return true;
            return false;
        }

        public static bool CheckFileNameExtension(string extension)
        {
            extension = extension.ToLower().Substring(1);
            if (extension.Equals(nameof(Constants.png)) || extension.Equals(nameof(Constants.jpeg)) || extension.Equals(nameof(Constants.jpeg2)) || extension.Equals(nameof(Constants.jpg)) || extension.Equals(nameof(Constants.tiff)) || extension.Equals(nameof(Constants.tiff2)) || extension.Equals(nameof(Constants.tiff)) || extension.Equals(nameof(Constants.jfif)) || extension.Equals(nameof(Constants.tif)))
            {
                return true;
            }
            return false;
        }

        public static string CreateObjectName(string fileName, string id)
        {
            return id + "." + GetFileNameExtension(fileName);
        }
        public static string CreateRoomObjectName(string fileName)
        {
            string RandomGen = Guid.NewGuid().ToString();
            return RandomGen+ "." + GetFileNameExtension(fileName);
        }
        public static string GetFileNameExtension(string fileName)
        {
            var extension = fileName.Split(".");
            return extension[extension.Length - 1];
        }

    }
}
