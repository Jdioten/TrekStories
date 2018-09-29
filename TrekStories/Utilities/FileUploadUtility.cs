using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TrekStories.Utilities
{
    public static class FileUploadUtility
    {
        private const int MAX_FILESIZE = 7000 * 1024;  //7MB converted in bytes

        private static string[] supportedFileExtensions = new[] { ".png", ".jpg", ".gif", ".pdf", ".msg", ".txt" };

        public static bool InvalidFileExtension(HttpPostedFileBase file)
        {
            string extension = Path.GetExtension(file.FileName);
            if (supportedFileExtensions.Contains(extension.ToLower()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool InvalidFileSize(HttpPostedFileBase file)
        {
            int size = file.ContentLength;
            if (size > MAX_FILESIZE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static String GetFilenameWithTimestamp(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename) + "_" + FileUploadUtility.GetTimestamp(DateTime.Now) + Path.GetExtension(filename);
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmss");
        }
    }
}