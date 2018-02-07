using MCRepositoryTool.Logic.Models;
using MCRepositoryTool.Logic.Repositories;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCRepositoryTool.ViewModels
{
    public class ProfileViewModel : BindableBase
    {
        readonly protected IEventAggregator EventAggregator;
        readonly protected MinecraftRepository MinecraftRepository;
        readonly protected Profile Profile;
        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        public ProfileViewModel(IEventAggregator ea, MinecraftRepository mr, Profile pr)
        {
            EventAggregator = ea;
            MinecraftRepository = mr;
            Profile = pr;
            IsExistFolder = Directory.Exists(Profile.gameDir);
            IsExistLogFolder = Directory.Exists(Profile.LogFolderPath);
            IsExistMergedLog = File.Exists(Profile.MergedLogFilePath);
            NotificationRequest = new InteractionRequest<INotification>();
            OpenRefreshedMergedLogCommand = new DelegateCommand(async() => await OpenRefreshedMergedLogAsync(), CanOpenRefreshedMergedLog);
            OpenMergedLogCommand = new DelegateCommand(OpenMergedLog, CanOpenMergedLog);
            OpenFolderCommand = new DelegateCommand(OpenFolder, CanOpenFolder);

        }

        public string Name
        {
            get
            {
                switch (Profile.type)
                {
                    case "latest-release":
                        return "最新バージョン";
                    case "latest-snapshot":
                        return "最新スナップショット";
                    default:
                        return Profile.name;
                }
            }
        }

        public string GameDir
        {
            get
            {
                return Profile.gameDir;
            }
        }

        public string LastVersionId
        {
            get
            {
                return Profile.lastVersionId;
            }
        }

        private bool _isExistFolder;
        public bool IsExistFolder
        {
            get
            {
                return _isExistFolder;
            }
            private set
            {
                SetProperty(ref _isExistFolder, value);
            }
        }

        private bool _isExistLogFolder;
        public bool IsExistLogFolder
        {
            get
            {
                return _isExistLogFolder;
            }
            private set
            {
                SetProperty(ref _isExistLogFolder, value);
            }
        }

        private bool _isExistMergedLog;
        public bool IsExistMergedLog
        {
            get
            {
                return _isExistMergedLog;
            }
            private set
            {
                SetProperty(ref _isExistMergedLog, value);
            }
        }

        public async Task RefreshMergedLogAsync()
        {
            await MinecraftRepository.MergeLogsAsync(Profile);
        }

        public DelegateCommand OpenMergedLogCommand
        {
            get;
            private set;
        }

        public void OpenMergedLog()
        {
            // ファイルを開く
            System.Diagnostics.Process.Start(Profile.MergedLogFilePath);
        }

        public bool CanOpenMergedLog()
        {
            return IsExistMergedLog;
        }

        public DelegateCommand OpenRefreshedMergedLogCommand
        {
            get;
            private set;
        }

        public async Task OpenRefreshedMergedLogAsync()
        {
            try
            {
                // 更新処理
                await RefreshMergedLogAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // ファイルを開く
                OpenMergedLog();
            }
        }

        public bool CanOpenRefreshedMergedLog()
        {
            return IsExistLogFolder;
        }

        public DelegateCommand OpenFolderCommand
        {
            get;
            private set;
        }

        public void OpenFolder()
        {
            System.Diagnostics.Process.Start(Profile.gameDir);
        }

        public bool CanOpenFolder()
        {
            return IsExistFolder;
        }

        private string _body;
        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                SetProperty(ref _body, value);
            }
        }
    }
}
