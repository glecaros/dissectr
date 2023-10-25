using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Dissectr.Util;

namespace Dissectr.ViewModels;

internal partial class MainViewModel: ObservableObject, IMediaControl
{
    public TimeSpan Position { get; set; }
    public TimeSpan Duration { get; set; }

    public bool IsPlaying { get; set; }

    public ICommand OnPlay { get; }
    public ICommand OnPause { get; }
    public ICommand OnStop{ get; }
    public ICommand OnSeek { get; }

    public MainViewModel()
    {
        OnPlay = new RelayCommand(PlayHandler);
        OnPause = new RelayCommand(PauseHandler);
        OnSeek = new RelayCommand<TimeSpan>(SeekHandler);
    }

    #region IMediaControl
    public event Action Play;
    public event Action Pause;
    public event Action<TimeSpan> Seek;
    #endregion

    private void RefreshProperties()
    {
        OnPropertyChanged(nameof(Position));
        OnPropertyChanged(nameof(Duration));
        OnPropertyChanged(nameof(IsPlaying));
    }

    private void PlayHandler()
    {
        if (!IsPlaying)
        {
            Play.Invoke();
        }
        IsPlaying = true;
    }

    private void PauseHandler()
    {
        if (IsPlaying)
        {
            Pause.Invoke();
        }
        IsPlaying = false;

    }

    private void SeekHandler(TimeSpan position)
    {
        Seek.Invoke(position);
    }


}