using System.Windows.Input;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Dissectr.Util;

namespace Dissectr.ViewModels;

internal partial class MainViewModel: ObservableObject, IMediaControl
{
    private TimeSpan position;
    public TimeSpan Position
    {
        get => position;
        set => SetProperty(ref position, value);
    }

    private TimeSpan duration;
    public TimeSpan Duration
    {
        get => duration;
        set => SetProperty(ref duration, value);
    }

    private MediaElementState playbackState;
    public MediaElementState PlaybackState
    {
        get => playbackState;
        set => SetProperty(ref playbackState, value);
    }

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
        OnPropertyChanged(nameof(PlaybackState));
    }

    private void PlayHandler()
    {
        if (PlaybackState is not MediaElementState.Playing)
        {
            Play.Invoke();
        }
    }

    private void PauseHandler()
    {
        if (PlaybackState is MediaElementState.Playing)
        {
            Pause.Invoke();
        }
    }

    private void SeekHandler(TimeSpan position)
    {
        Seek.Invoke(position);
    }


}