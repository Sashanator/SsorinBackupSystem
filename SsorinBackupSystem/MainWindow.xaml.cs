using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text.RegularExpressions;

namespace LabWork_4__REWORKED_
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> _filePaths;
        private Dictionary<int, List<string>> _backupFiles;
        private int _fileID = 1;
        private int _backupID = 1;
        private List<Backup> _backups;
        private int _n = 0; // #1
        private int _seconds = 0; // #2
        private double _size = 0; // #3

        public MainWindow()
        {
            InitializeComponent();

            _backups = new List<Backup>();
            _backupFiles = new Dictionary<int, List<string>>();
            _filePaths = new List<string>();
        }

        private void FindFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.ShowDialog();
            FilePath.Text = openFile.FileName;
        }

        private void AddFile(object sender, RoutedEventArgs e)
        {
            if (FilePath.Text.Length > 0) {
                _filePaths.Add(FilePath.Text);

                string FileName = MySubString(FilePath.Text, FilePath.Text.LastIndexOf(@"\") + 1,
                                    FilePath.Text.Length);
                ListFiles.Items.Add(_fileID++ + ". " + FileName);
                FilePath.Text = "";
            } else {
                MessageBox.Show("Выберете файл!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        private void CreateBackup(object sender, RoutedEventArgs e)
        { // Создание директории и копирование туда выбранных файлов
            string curPath = $"{App.LOCAL_PATH}_{_backupID}";
            Directory.CreateDirectory(curPath);
            foreach (var f in _filePaths) {
                string FileName = MySubString(f, f.LastIndexOf(@"\") + 1,
                                    f.Length);
                File.Copy(f, $@"{curPath}\{FileName}");
            }

            List<FileData> files = new List<FileData>();
            foreach (var f in Directory.GetFiles(curPath)) {
                FileInfo fileInfo = new FileInfo(f);
                files.Add(new FileData(f, fileInfo.Length, File.GetLastWriteTime(f)));
            }

            // Add to ListBox 
            ListBoxItem item = new ListBoxItem();
            item.Content = $"Backup #{_backupID} | Time: {DateTime.Now.ToString("HH:mm:ss")} | Files: {files.Count}";
            BackupList.Items.Add(item);

            // Add to list
            _backupFiles.Add(_backupID, new List<string>(_filePaths));
            if (UnionAlg.IsChecked == true) {
                _backups.Add(new Backup(_backupID++, DateTime.Now, files, true));
            } else if (GroupAlg.IsChecked == true) {
                _backups.Add(new Backup(_backupID++, DateTime.Now, files, false));
            } else {
                MessageBox.Show("Error", "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateBackup(object sender, RoutedEventArgs e)
        {
            string sub = BackupList.SelectedItem.ToString();
            int id = GetSelectedID(sub) + 1;

            string curPath = $"{App.LOCAL_PATH}_{id}";

            if (Directory.Exists(curPath)) {
                Directory.Delete(curPath, true);
            }
            Directory.CreateDirectory(curPath);

            foreach (var f in ListFiles.Items) {
                string file = f.ToString();
                string FileName = MySubString(file, file.IndexOf(".") + 2,
                                    file.Length);
                File.Copy($@"{App.DATA_PATH}\{FileName}", $@"{curPath}\{FileName}");
            }

            List<FileData> files = new List<FileData>();
            foreach (var f in Directory.GetFiles(curPath)) {
                FileInfo fileInfo = new FileInfo(f);
                files.Add(new FileData(f, fileInfo.Length, File.GetLastWriteTime(f)));
            }
            // Update ListBox
            for (int i = 0; i < BackupList.Items.Count; i++) {
                string item = BackupList.Items[i].ToString();
                int curID = Convert.ToInt32(MySubString(item, item.IndexOf('#') + 1, item.IndexOf('|') - 1));
                if (curID == id) {
                    BackupList.Items[i] = $"Backup #{id} | Time: {DateTime.Now.ToString("HH:mm:ss")} | Files: {files.Count}";
                    break;
                }
            }

            _backups[id - 1].AddFiles(files);
        }

        private void DeletePoints(object sender, RoutedEventArgs e)
        {
            List<int> commands = new List<int>();
            if (CheckBoxCount.IsChecked == true) {
                commands.Add(1);
            }
            if (CheckBoxDate.IsChecked == true) {
                commands.Add(2);
            }
            if (CheckBoxSize.IsChecked == true) {
                commands.Add(3);
            }

            string sub = BackupList.SelectedItem.ToString();
            int id = GetSelectedID(sub);

            List<int> ids = new List<int>();
            if (RadioAll.IsChecked == true) {
                ids = new List<int>(_backups[id].DeletePointsAllCombo(commands, _n,
                    DateTime.Now.AddSeconds(-_seconds), _size));
            } else if (RadioAny.IsChecked == true) {
                ids = new List<int>(_backups[id].DeletePointsAnyCombo(commands, _n,
                    DateTime.Now.AddSeconds(-_seconds), _size));
            } else {
                MessageBox.Show("Error", "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try {
                _backups[id].DeletePoints(ids);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
                int idNotToDelete = Convert.ToInt32(
                        MySubString(ex.Message, ex.Message.IndexOf(':') + 1, ex.Message.Length));
                for (int i = 0; i < ids.Count; i++) {
                    if (ids[i] == idNotToDelete) {
                        ids.Remove(ids[i]);
                        break;
                    }
                }
            }
            foreach (var i in ids) { // Удалить физические копии
                Regex pattern = new Regex($@"RestorePoint_{i}(\w*)");
                var files = Directory.GetFiles($"{App.DESTINATION_PATH}_{id + 1}")
                                     .Where(path => pattern.IsMatch(path));
                foreach (var f in files) {
                    if (File.Exists(f)) {
                        File.Delete(f);
                    }
                }
            }

        }

        private void CheckAlgorithmCount(object sender, RoutedEventArgs e)
        {
            var dialog = new MyDialog();
            if (dialog.ShowDialog() == true) {
                if (CheckBoxCount.IsChecked == true) {
                    _n = Convert.ToInt32(dialog.ResponseText);
                }
            }
        }

        private void CheckAlgorithmDate(object sender, RoutedEventArgs e)
        {
            var dialog = new MyDialog();
            if (dialog.ShowDialog() == true) {
                if (CheckBoxDate.IsChecked == true) {
                    _seconds = Convert.ToInt32(dialog.ResponseText);
                }
            }
        }

        private void CheckAlgorithmSize(object sender, RoutedEventArgs e)
        {
            var dialog = new MyDialog();
            if (dialog.ShowDialog() == true) {
                if (CheckBoxSize.IsChecked == true) {
                    _size = Convert.ToDouble(dialog.ResponseText);
                }
            }
        }

        private void SelectBackup(object sender, RoutedEventArgs e)
        {
            ListFiles.Items.Clear();
            _fileID = 1;
            _filePaths.Clear();

            string sub = BackupList.SelectedItem.ToString();
            int id = GetSelectedID(sub) + 1;
            foreach (var f in _backupFiles[id]) {
                string FileName = MySubString(f, f.LastIndexOf(@"\") + 1, f.Length);
                ListFiles.Items.Add(_fileID++ + ". " + FileName);
            }
        }

        private void CreateRestorePoint(object sender, RoutedEventArgs e)
        {
            string sub = BackupList.SelectedItem.ToString();
            int id = GetSelectedID(sub);
            _backups[id].AddRestorePoint();
        }

        private void CreateIncRestorePoint(object sender, RoutedEventArgs e)
        {
            string sub = BackupList.SelectedItem.ToString();
            int id = GetSelectedID(sub);
            _backups[id].AddIncRestorePoint();
        }

        private void DeletePointsByCount(object sender, RoutedEventArgs e)
        {
            string sub = BackupList.SelectedItem.ToString();
            int id = GetSelectedID(sub);
            var dialog = new MyDialog();
            if (dialog.ShowDialog() == true) {
                var ids = _backups[id].DeletePointsByCount(Convert.ToInt32(dialog.ResponseText));
                try {
                    _backups[id].DeletePoints(ids);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    int idNotToDelete = Convert.ToInt32(
                        MySubString(ex.Message, ex.Message.IndexOf(':') + 1, ex.Message.Length));
                    for (int i = 0; i < ids.Count; i++) {
                        if (ids[i] == idNotToDelete) {
                            ids.Remove(ids[i]);
                            break;
                        }
                    }
                }

                foreach (var i in ids) { // Удалить физические копии
                    Regex pattern = new Regex($@"RestorePoint_{i}(\w*)");
                    var files = Directory.GetFiles($"{App.DESTINATION_PATH}_{id + 1}")
                                         .Where(path => pattern.IsMatch(path));
                    foreach (var f in files) {
                        if (File.Exists(f)) {
                            File.Delete(f);
                        }
                    }
                }
            }
        }

        private void DeletePointsByDate(object sender, RoutedEventArgs e)
        {
            string sub = BackupList.SelectedItem.ToString();
            int id = GetSelectedID(sub);
            var dialog = new MyDialog();
            if (dialog.ShowDialog() == true) {
                var ids = _backups[id].DeletePointsByDate(
                    DateTime.Now.AddSeconds(-Convert.ToInt32(dialog.ResponseText))); // Для наглядности
                try {
                    _backups[id].DeletePoints(ids);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    int idNotToDelete = Convert.ToInt32(
                        MySubString(ex.Message, ex.Message.IndexOf(':') + 1, ex.Message.Length));
                    for (int i = 0; i < ids.Count; i++) {
                        if (ids[i] == idNotToDelete) {
                            ids.Remove(ids[i]);
                            break;
                        }
                    }
                }

                foreach (var i in ids) { // Удалить физические копии
                    Regex pattern = new Regex($@"RestorePoint_{i}(\w*)");
                    var files = Directory.GetFiles($"{App.DESTINATION_PATH}_{id + 1}")
                                         .Where(path => pattern.IsMatch(path));
                    foreach (var f in files) {
                        if (File.Exists(f)) {
                            File.Delete(f);
                        }
                    }
                }
            }
        }

        private void DeletePointsBySize(object sender, RoutedEventArgs e)
        {
            string sub = BackupList.SelectedItem.ToString();
            int id = GetSelectedID(sub);
            var dialog = new MyDialog();
            if (dialog.ShowDialog() == true) {
                var ids = _backups[id].DeletePointsBySize(Convert.ToInt32(dialog.ResponseText));
                try {
                    _backups[id].DeletePoints(ids);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    int idNotToDelete = Convert.ToInt32(
                        MySubString(ex.Message, ex.Message.IndexOf(':') + 1, ex.Message.Length));
                    for (int i = 0; i < ids.Count; i++) {
                        if (ids[i] == idNotToDelete) {
                            ids.Remove(ids[i]);
                            break;
                        }
                    }
                }

                foreach (var i in ids) { // Удалить физические копии
                    Regex pattern = new Regex($@"RestorePoint_{i}(\w*)");
                    var files = Directory.GetFiles($"{App.DESTINATION_PATH}_{id + 1}")
                                         .Where(path => pattern.IsMatch(path));
                    foreach (var f in files) {
                        if (File.Exists(f)) {
                            File.Delete(f);
                        }
                    }
                }
            }
        }

        private void ClearList(object sender, RoutedEventArgs e)
        {
            ListFiles.Items.Clear();
            _fileID = 1;
            _filePaths.Clear();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Regex patternLocal = new Regex($@"(\w*){App.LOCAL_PATH}(\w*)");
            var dirsProg = Directory.GetDirectories(Directory.GetCurrentDirectory())
                                .Where(path => patternLocal.IsMatch(path))
                                .ToList();
            foreach (var d in dirsProg) {
                if (Directory.Exists(d)) {
                    Directory.Delete(d, true);
                }
            }

            Regex patternZip = new Regex($@"(\w*){App.DESTINATION_PATH}(\w*)");
            var dirsZip = Directory.GetDirectories(Directory.GetCurrentDirectory())
                                .Where(path => patternZip.IsMatch(path))
                                .ToList();
            foreach (var d in dirsZip) {
                if (Directory.Exists(d)) {
                    Directory.Delete(d, true);
                }
            }
            this.Close();
        }

        // Help methods

        private int GetSelectedID(string sub)
            => Convert.ToInt32(MySubString(sub, sub.IndexOf("#") + 1, sub.IndexOf("|") - 1)) - 1;

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
