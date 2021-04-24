using LabWork_4__REWORKED_.Deliters;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace LabWork_4__REWORKED_
{
    public class Backup
    {
        public int ID { get; set; }
        public bool IsUnionAlgorithm { get; set; } // true = Единый алгоритм, false = Разделённый
        public DateTime CreationTime { get; set; }
        public double BackupSize { get; set; }
        public List<FileData> Files { get; set; }
        public List<RestorePoint> Points { get; set; }
        // Deliters
        private DeliterByCount deliterByCount_ = new DeliterByCount();
        private DeliterByDate deliterByDate_ = new DeliterByDate();
        private DeliterBySize deliterBySize_ = new DeliterBySize();

        private int pID_ = 1;

        public Backup(int id, DateTime creationTime, List<FileData> files, bool isUnion)
        {
            ID = id;
            CreationTime = creationTime;
            IsUnionAlgorithm = isUnion;

            Points = new List<RestorePoint>();

            AddFiles(files);
        }

        public List<int> DeletePointsByCount(int n) // #1
            => deliterByCount_.DeleteByCriterion(n, Points);

        public List<int> DeletePointsByDate(DateTime date) // #2
            => deliterByDate_.DeleteByCriterion(date, Points);

        public List<int> DeletePointsBySize(double kb) // #3
            => deliterBySize_.DeleteByCriterion(kb, Points);

        public List<int> DeletePointsAnyCombo(List<int> commands,  // #4.1
            int n = 0, DateTime date = new DateTime(), double size = 0)
        {
            HashSet<int> pointsToDelete = new HashSet<int>();

            foreach (var i in commands) {
                switch (i) {
                    case 1:
                        foreach (var id in DeletePointsByCount(n)) {
                            pointsToDelete.Add(id);
                        }
                        break;
                    case 2:
                        foreach (var id in DeletePointsByDate(date)) {
                            pointsToDelete.Add(id);
                        }
                        break;
                    case 3:
                        foreach (var id in DeletePointsBySize(size)) {
                            pointsToDelete.Add(id);
                        }
                        break;
                }
            }
            return pointsToDelete.ToList();
        }

        public List<int> DeletePointsAllCombo(List<int> commands, // #4.2
            int n = 0, DateTime date = new DateTime(), double size = 0)
        {
            List<int> list = new List<int>();
            foreach (var i in commands) {
                switch (i) {
                    case 1:
                        list.AddRange(DeletePointsByCount(n));
                        break;
                    case 2:
                        list.AddRange(DeletePointsByDate(date));
                        break;
                    case 3:
                        list.AddRange(DeletePointsBySize(size));
                        break;
                }
            }
            var result = list.GroupBy(x => x)
                             .ToDictionary(y => y.Key, y => y.Count())
                             .OrderByDescending(z => z.Value);
            List<int> pointsToDelete = new List<int>();
            foreach (var x in result) {
                if (x.Value == commands.Count) {
                    pointsToDelete.Add(x.Key);
                }
            }
            return pointsToDelete;
        }

        public void DeletePoints(List<int> pointsToDelete)
        {
            for (int i = 0; i < Points.Count; i++) {
                bool flag = true;
                if (pointsToDelete.Contains(Points[i].ID)) {
                    // #1: Is Full?
                    if (Points[i].IsFull) {
                        // #2: Find children by pattern
                        Regex pattern = new Regex($@"(\w*)I{Points[i].ID}(\w*)");
                        var files = Directory.GetFiles($@"{App.DESTINATION_PATH}_{ID}")
                                             .Where(path => pattern.IsMatch(path));
                        // #3: Find childrens' ID
                        foreach (var f in files) {
                            int id =
                                Convert.ToInt32(MySubString(f, f.LastIndexOf("_") + 1, f.LastIndexOf("I")));
                            // #4: Если выполняется, значит они не удалены
                            if (!pointsToDelete.Contains(id)) {
                                flag = false;
                                throw new Exception($"Данная точка имеет инкрементальных детей: {Points[i].ID}");
                            }
                        }
                    }
                    if (flag) { Points.Remove(Points[i--]); }
                }
            }
        }

        public void AddFiles(List<FileData> newFiles)
        {
            Files = new List<FileData>();
            Files.AddRange(newFiles);
            BackupSize = 0;
            foreach (var i in Files) {
                BackupSize += i.FileSize;
            }
        }
        // TODO: Update Backup Files!! #1
        // TODO: Создание нескольких точек восстановления #2 READY
        public void AddRestorePoint()
        {
            RestorePoint point = new RestorePoint(pID_++, true, DateTime.Now, BackupSize, Files);
            Points.Add(point);

            if (!Directory.Exists($"{App.DESTINATION_PATH}_{ID}")) {
                Directory.CreateDirectory($"{App.DESTINATION_PATH}_{ID}");
            }

            var zipFile = $@"{App.DESTINATION_PATH}_{ID}\RestorePoint_{point.ID}.zip";

            if (IsUnionAlgorithm) { // Объединённый алгоритм
                using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create)) {
                    foreach (var f in Files) {
                        archive.CreateEntryFromFile(f.FilePath, Path.GetFileName(f.FilePath));
                    }
                }
            } else { // Разъединённый алгоритм
                string tmpPath = "KeepingDirs";
                foreach (var f in Files) {
                    // Создаём папку для каждого файла с именем файла
                    string path = $@"{tmpPath}\{Path.GetFileNameWithoutExtension(f.FilePath)}";
                    Directory.CreateDirectory(path);
                    // Копируем каждый файл в соотв. папку
                    File.Copy(f.FilePath, $@"{path}\{Path.GetFileName(f.FilePath)}");
                }
                ZipFile.CreateFromDirectory(tmpPath, zipFile);
                Directory.Delete(tmpPath, true);
            }
        }

        public void AddIncRestorePoint()
        {
            // 1. Берём последнюю полную точку
            RestorePoint lastFullPoint = Points.Where(p => p.IsFull == true).Last();
            // 2. Добавляем новые и изменившиеся файлы
            List<FileData> incFiles = new List<FileData>();
            foreach (var fBackup in Files) {
                if (lastFullPoint.Files.Where(f => f.FilePath == fBackup.FilePath).ToList().Count == 0) {
                    incFiles.Add(fBackup); // добавляем его в дельту
                } else { // Если файл был в ТВ, то находим этот файл и чекаем дату и размер
                    foreach (var fPoint in lastFullPoint.Files) {
                        if (fBackup.FilePath == fPoint.FilePath) {
                            if (fBackup.FileSize != fPoint.FileSize ||
                                fBackup.FileDate != fPoint.FileDate) {
                                incFiles.Add(fBackup);
                            }
                        }
                    }
                }
            }
            // 3. Ищем новый поинт сайз
            double pointSize = 0;
            foreach (var f in incFiles) {
                pointSize += f.FileSize;
            }
            // 4. Добавляем новую инкрементную точку
            RestorePoint point = new RestorePoint(pID_++, false, DateTime.Now, pointSize, incFiles);
            Points.Add(point);

            // 5. New Zip File
            var zipFile = $@"{App.DESTINATION_PATH}_{ID}\RestorePoint_{point.ID}I{lastFullPoint.ID}.zip";
            // 6. Poehali!
            if (IsUnionAlgorithm) { // Объединённый алгоритм
                using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create)) {
                    foreach (var f in incFiles) {
                        archive.CreateEntryFromFile(f.FilePath, Path.GetFileName(f.FilePath));
                    }
                }
            } else { // Разъединённый алгоритм
                string tmpPath = "KeepingDirs";
                foreach (var f in incFiles) {
                    // Создаём папку для каждого файла с именем файла
                    string path = $@"{tmpPath}\{Path.GetFileNameWithoutExtension(f.FilePath)}";
                    Directory.CreateDirectory(path);
                    // Копируем каждый файл в соотв. папку
                    File.Copy(f.FilePath, $@"{path}\{Path.GetFileName(f.FilePath)}");
                }
                ZipFile.CreateFromDirectory(tmpPath, zipFile);
                Directory.Delete(tmpPath, true);
            }
        }

        // Help methods
        public static string MySubString(string source, int start, int end)
        {
            string result = "";
            for (int i = start; i < end; i++) {
                result += source[i];
            }
            return result;
        }
    }
}
