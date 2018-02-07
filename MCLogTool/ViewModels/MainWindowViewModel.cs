using MCRepositoryTool.Logic.Events;
using MCRepositoryTool.Logic.Models;
using MCRepositoryTool.Logic.Repositories;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCLogTool.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IEventAggregator EventAggregator;
        private readonly MinecraftRepository MinecraftRepository;

        public MainWindowViewModel(IEventAggregator ea, MinecraftRepository mr)
        {
            EventAggregator = ea;
            MinecraftRepository = mr;
            Errors = new ObservableCollection<ErrorViewModel>();
            MergeLogCommand = new DelegateCommand<ProfileViewModel>(async(p) => await MergeLogAsync(p));
            EventAggregator.GetEvent<ProgressNotificationEvent>().Subscribe(OnProgress, ThreadOption.UIThread);
            EventAggregator.GetEvent<ProfilesRefreshEvent>().Subscribe(async (param) => await RefreshProfilesAsync(), ThreadOption.UIThread);
            EventAggregator.GetEvent<ErrorNotificationEvent>().Subscribe(AddError, ThreadOption.UIThread);
            ErrorClearCommand = new DelegateCommand(ClearError);
            Task.Run(async () => await RefreshProfilesAsync());
        }

        #region Profiles

        public string RepositoryFolder
        {
            get => MinecraftRepository.RepositoryFolder;
        }

        private async Task RefreshProfilesAsync()
        {
            try
            {
                Profiles = new ObservableCollection<ProfileViewModel>((from p in await MinecraftRepository.GetProfilesAsync()
                                                                       orderby p.name descending
                                                                       select new ProfileViewModel(EventAggregator, MinecraftRepository, p)).ToList());
            }
            catch(Exception)
            {
                Profiles = new ObservableCollection<ProfileViewModel>();
            }
        }

        private ObservableCollection<ProfileViewModel> _profiles;
        public ObservableCollection<ProfileViewModel> Profiles
        {
            get
            {
                return _profiles;
            }
            set
            {
                SetProperty(ref _profiles, value);
            }
        }

        private ProfileViewModel _selectedProfile;
        public ProfileViewModel SelectedProfile
        {
            get
            {
                return _selectedProfile;
            }
            set
            {
                SetProperty(ref _selectedProfile, value);
            }
        }

        public DelegateCommand<ProfileViewModel> MergeLogCommand
        {
            get;
            private set;
        }

        private async Task MergeLogAsync(ProfileViewModel profile)
        {
            if (profile == null) return;
            if (!profile.CanOpenRefreshedMergedLog()) return;

            try
            {
                IsWorking = true;
                Errors.Clear();
                await profile.OpenRefreshedMergedLogAsync();
            }
            catch(Exception)
            {
            }
            finally
            {
                SelectedProfile = null;
                IsWorking = false;
            }
        }

        #endregion

        #region Work

        private bool _isWorking;
        public bool IsWorking
        {
            get
            {
                return _isWorking;
            }
            set
            {
                SetProperty(ref _isWorking, value);

                if(!_isWorking)
                {
                    Progress = 0;
                    Maximum = 0;
                    Value = 0;
                    Title = null;
                    Message = null;
                }
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _meessage;
        public string Message
        {
            get => _meessage;
            set => SetProperty(ref _meessage, value);
        }

        private long _maximum;
        public long Maximum
        {
            get => _maximum;
            set => SetProperty(ref _maximum, value);
        }

        private long _value;
        public long Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        private double _progress;
        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        private void OnProgress(Progress progress)
        {
            Progress = (double)progress.Value / progress.Maximum;
            Maximum = progress.Maximum;
            Value = progress.Value;
            Title = progress.Title;
            Message = progress.Message;
        }

        private ObservableCollection<ErrorViewModel> _errors;
        public ObservableCollection<ErrorViewModel> Errors
        {
            get => _errors;
            set => SetProperty(ref _errors, value);
        }

        public DelegateCommand ErrorClearCommand
        {
            get;
            private set;
        }

        public void ClearError()
        {
            Errors.Clear();
        }

        public void AddError(Error error)
        {
            if (error == null) throw new ArgumentNullException("error");
            Errors.Add(new ErrorViewModel(error));
        }

        #endregion
    }
}
