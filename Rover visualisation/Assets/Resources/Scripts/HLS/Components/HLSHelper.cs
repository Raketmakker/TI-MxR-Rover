using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.HLS
{
    public class HLSHelper
    {

        // Singleton functions
        private HLSHelper() {}

        /**
         * @function: static getInstance()
         * @return: HLSHelper
         * @description: Always returns the same helper to prevent using memory for the helper in all commands.
         */
        public static HLSHelper getInstance()
        {

            if (instance == null)
                instance = new HLSHelper();

            return instance; 
        }

        // Singleton
        private static HLSHelper instance;

        // Helper functions

        /**
         * @function: downloadFileAsync()
         * @param: AsyncCompletedEventHandler callback,
         * @param: string url,
         * @param: string filename,
         * @param: string path,
         * @param: string downloadFilename = null
         * @description: This functions downloads a specific file from the internet.
         */
        public void downloadFileAsync(AsyncCompletedEventHandler callback, string url, string filename, string path, string downloadFilename = null)
        {

            if (downloadFilename == null)
                downloadFilename = filename;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (WebClient WC = new WebClient())
            {

                WC.DownloadFileCompleted += callback;
                WC.DownloadFileAsync(new Uri(url + '/' + filename), path + '\\' + downloadFilename);
            }
        }

        /**
         * @function: searchFileForLines()
         * @param: string filename,
         * @param: string path,
         * @param: string search = null,
         * @param: bool mustContain = true
         * @return: List<string>
         * @description: returns all the lines in a given file containing the requested information.
         */
        public List<string> searchFileForLines(string filename, string path, string search = null, bool mustContain = true)
        {

            StreamReader fileReader = new StreamReader(path + '\\' + filename);

            List<string> lines = new List<string>();
            string line;

            while ((line = fileReader.ReadLine()) != null)
                if (line.Length > 0 && (search == null || mustContain && line.Contains(search) || !mustContain && !line.Contains(search)))
                    lines.Add(line);

            return lines;
        }

        /**
         * @function: getFileSize()
         * @param: string filename,
         * @param: string path,
         * @return: long
         * @description: This function returns the size of a file.
         */
        public long getFileSize(string filename, string path)
        {

            return new FileInfo(path + "\\" + filename).Length;
        }

        /**
         * @function: isDownloaded()
         * @param: string filename,
         * @param: string path,
         * @param: long size         = -1
         * @param: long originalSize = -1
         * @return: bool
         * @description: 
         */
        public bool isDownloaded(string filename, string path, long size = -1, long originalSize = -1)
        {

            if (size == -1)
                size = this.getFileSize(filename, path);

            return (size > 0 && (originalSize == -1 || originalSize == size));
        }

        /**
         * @function: removeExtension()
         * @param: string filename
         * @return: string
         * @description: This function removes the extention from the filename
         */
        public string removeExtension(string filename)
        {

            string newFilename = "";
            string[] filenameParts = filename.Split('.');

            for (int i = 0; i < filenameParts.Length - 1; i++)
                newFilename += filenameParts[i];

            return newFilename;
        }

        /**
         * @function: replaceExtension()
         * @param: string filename,
         * @param: string extension
         * @return: string
         * @description: This function replaces the file extension of the filename.
         */
        public string replaceExtension(string filename, string extension)
        {

            if (!extension.Contains('.'))
                extension = "." + extension;

            return this.removeExtension(filename) + extension;
        }

        /**
         * @function: clearPath()
         * @param: string path
         * @description: This function prevents the removal of files in unwanted folders. 
         * Only the HLSDownloads folder can be cleared.
         */
        public void clearPath(string path)
        {

            path += "\\HLSDownloads";

            if (Directory.Exists(path))
                this.clearDirectory(new DirectoryInfo(path));
        }

        /**
        * @function: clearPath()
        * @param: DirectoryInfo directory
        * @description: This function removes all files and folders of a 
        */
        private void clearDirectory(DirectoryInfo directory)
        {
            
            foreach (DirectoryInfo childDirectory in directory.GetDirectories())
                this.clearDirectory(childDirectory);

            foreach (FileInfo file in directory.GetFiles())
                file.Delete();

            directory.Delete();
        }
    }
}
