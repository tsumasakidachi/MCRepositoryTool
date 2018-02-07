using MCRepositoryTool.Logic.Events;
using MCRepositoryTool.Logic.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCRepositoryTool.Logic.Repositories
{
    public class MinecraftRepository : IMinecraftRepository
    {
        readonly protected IEventAggregator EventAggregator;
        public Encoding MergedLogEncoding = new UTF8Encoding(false);
        public Encoding MinecraftEncoding = Encoding.GetEncoding("shift_jis");

        public MinecraftRepository(IEventAggregator ea)
        {
            EventAggregator = ea;

            if (File.Exists(ProfileFilePath))
            {
                ProfileWatcher = new FileSystemWatcher(RepositoryFolder)
                {
                    Filter = ProfileFileName,
                    NotifyFilter = NotifyFilters.LastWrite
                };
                ProfileWatcher.Changed += ProfileWatcher_Changed;
                ProfileWatcher.EnableRaisingEvents = true;
            }
        }

        #region Repository and profiles

        public static string RepositoryFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.minecraft";
        public static string ProfileFileName = "launcher_profiles.json";
        public static string ProfileFilePath = RepositoryFolder + @"\" + ProfileFileName;
        public static string LogFolderName = @"\logs";
        public static string MergedLogFileName = @"\merged.log";
        protected FileSystemWatcher ProfileWatcher { get; set; }

        private void ProfileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            System.Threading.Thread.Sleep(500);
            EventAggregator.GetEvent<ProfilesRefreshEvent>().Publish(null);
        }

        /*
        public string SelectProfile()
        {
            var custom = new OpenFileDialog();
            custom.Title = "launcher_profile.json の選択";
            custom.CheckFileExists = true;
            custom.CheckPathExists = true;
            custom.DereferenceLinks = true;
            custom.Filter = "JSON ファイル|*.json";
            custom.Multiselect = false;
            custom.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var result = custom.ShowDialog();

            if (result == DialogResult.OK)
            {
                return custom.FileName;
            }
            else
            {
                throw new Exception();
            }
        }
        */

        public async Task<List<Profile>> GetProfilesAsync()
        {
            try
            {
                var json = await Task.Run(() => File.ReadAllText(ProfileFilePath, MinecraftEncoding));
                var serializer = new JavaScriptSerializer();
                var repository = serializer.Deserialize<Repository>(json);
                var profiles = repository.profiles.Select(x => x.Value).ToList();

                profiles.ForEach(p =>
                {
                    if (String.IsNullOrEmpty(p.gameDir)) p.gameDir = RepositoryFolder;
                    p.LogFolderPath = p.gameDir + LogFolderName;
                    p.MergedLogFilePath = p.LogFolderPath + MergedLogFileName;
                });

                return profiles;
            }
            catch (DirectoryNotFoundException ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Log Merging

        public async Task MergeLogsAsync(Profile profile)
        {
            // プロファイル フォルダが見つからなければ終了
            if (!Directory.Exists(profile.LogFolderPath)) throw new DirectoryNotFoundException();

            // ログ ファイルを収集
            var logs = await GetLogFilesAsync(profile.LogFolderPath);
            uint progress = 0;

            using (var outputFileStream = new FileStream(profile.MergedLogFilePath, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var writer = new StreamWriter(outputFileStream, MergedLogEncoding))
            {
                await Task.Run(async () =>
                {
                    foreach (var log in logs)
                    {
                        var logFileInfo = new FileInfo(log);

                        progress++;

                        if (progress % 4 == 0)
                        {
                            string[] suffix = { "", "K", "M", "G" };
                            int index = 0;
                            double size = logFileInfo.Length;
                            int p = 1024;

                            while (size >= p && index < suffix.Length)
                            {
                                size /= p;
                                index++;
                            }

                            RaiseWorkNortificationEvent("ファイルを結合中", logFileInfo.Name + string.Format(" ({0:F0}{1}B)", size, suffix[index]), logs.Count, progress);
                        }

                        try
                        {
                            foreach (var l in GetLines(logFileInfo))
                            {
                                // エンコードを変換する
                                var bytes = Encoding.Convert(MinecraftEncoding, MergedLogEncoding, MinecraftEncoding.GetBytes(l));
                                var line = MergedLogEncoding.GetString(bytes);

                                // 部品に分割する
                                var match = Regex.Match(line, @"^\[(\d{2})\:(\d{2})\:(\d{2})\] \[(.+?)\]\: (.+)$");

                                // チャット以外は無視して次へ行く
                                if (!Regex.IsMatch(match.Groups[5].Value, @"\[CHAT\]")) continue;

                                // ログの日付を作成する
                                var createdDate = GetLogFileCreatedDate(logFileInfo);
                                int hour = int.Parse(match.Groups[1].Value);
                                int minute = int.Parse(match.Groups[2].Value);
                                int second = int.Parse(match.Groups[3].Value);
                                var time = new TimeSpan(0, hour, minute, second);
                                var lineDateTime = createdDate.Add(time);

                                // 行の時刻を日付時刻に置き換える
                                line = line.Remove(0, 10);
                                line = "[" + lineDateTime.ToString("G") + "]" + line;

                                // 結合ファイルに書き込む
                                await writer.WriteLineAsync(line);
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseErrorNortificationEvent(new Error()
                            {
                                Type = ErrorType.Error,
                                FilePath = log,
                                Message = ex.Message
                            });
                        }
                    }
                });
            }
        }

        private IEnumerable<string> GetLines(FileInfo source)
        {
            string line = null;

                switch (source.Extension)
                {
                    case ".gz":
                        using (var compressedStream = source.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                        using (var stream = new GZipStream(compressedStream, CompressionMode.Decompress))
                        using (var reader = new StreamReader(stream, MinecraftEncoding))
                        {
                            while ((line = reader.ReadLine()) != null)
                            {
                                yield return line;
                            }
                        }
                        break;
                    case ".log":
                        using (var stream = source.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                        using (var reader = new StreamReader(stream, MinecraftEncoding))
                        {
                            while ((line = reader.ReadLine()) != null)
                            {
                                yield return line;
                            }
                        }

                        break;
                }
        }

        private string FilterLine(string line)
        {
            return line;
        }

        private DateTime GetLogFileCreatedDate(FileInfo source)
        {
            if (source.Name == "latest.log")
            {
                return source.LastWriteTime.Date;
            }
            else
            {
                var match = Regex.Match(source.Name, @"^(\d{4})\-(\d{2})\-(\d{2})\-(\d+)\.log(?:\.gz)?$");
                int year = int.Parse(match.Groups[1].Value);
                int month = int.Parse(match.Groups[2].Value);
                int date = int.Parse(match.Groups[3].Value);
                return new DateTime(year, month, date);
            }

        }

        private async Task<List<string>> GetLogFilesAsync(string folderPath)
        {
            var files = await Task.Run(() => Directory.GetFiles(folderPath).Where(x => Regex.IsMatch(x, @"\.log\.gz$")).ToList());
            files.Add(folderPath + @"\latest.log");

            return files;
        }

        #endregion

        #region Work

        private void RaiseWorkNortificationEvent(string title, string message, long maximum, long value)
        {
            Progress p = new Progress()
            {
                Title = title,
                Message = message,
                Maximum = maximum,
                Value = value
            };
            RaiseWorkNortificationEvent(p);
        }

        private void RaiseWorkNortificationEvent(Progress progress)
        {
            EventAggregator.GetEvent<ProgressNotificationEvent>().Publish(progress);
        }

        private void RaiseErrorNortificationEvent(Error error)
        {
            EventAggregator.GetEvent<ErrorNotificationEvent>().Publish(error);
        }

        #endregion
    }
}
