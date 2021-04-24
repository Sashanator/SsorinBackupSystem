using System;

namespace LabWork_4__REWORKED_
{
    public class FileData
    {
        public string FilePath { get; set; }
        public double FileSize { get; set; }
        public DateTime FileDate { get; set; }

        public FileData(string path, double size, DateTime date)
        {
            FilePath = path;
            FileSize = size;
            FileDate = date;
        }
    }
}
