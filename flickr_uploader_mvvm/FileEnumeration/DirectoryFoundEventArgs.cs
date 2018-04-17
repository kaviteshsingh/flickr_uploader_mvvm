using System;
using System.IO;


namespace FileEnumeration
{
    public class DirectoryFoundEventArgs: EventArgs
    {
        public DirectoryInfo directoryInfo { get; set; }

        public DirectoryFoundEventArgs(DirectoryInfo inDirectoryInfo)
        {
            directoryInfo = inDirectoryInfo;
        }
    }
}
