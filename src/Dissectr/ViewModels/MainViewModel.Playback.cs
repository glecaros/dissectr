using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using Dissectr.Util;

namespace Dissectr.ViewModels;

public partial class MainViewModel: ObservableObject, IMediaControl
{
    [ObservableProperty]
    private TimeSpan position;

    [ObservableProperty]
    private TimeSpan duration;

    [ObservableProperty]
    private MediaElementState playbackState;

    [ObservableProperty]
    private TimeSpan intervalLength = TimeSpan.Zero;

    public MainViewModel()
    {
    }

    #region IMediaControl
    public event Action? Play;
    public event Action? Pause;
    public event Action<TimeSpan>? Seek;
    #endregion

    private void RefreshProperties()
    {
        OnPropertyChanged(nameof(Position));
        OnPropertyChanged(nameof(Duration));
        OnPropertyChanged(nameof(PlaybackState));
    }

    [RelayCommand]
    private void OnPlay()
    {
        if (PlaybackState is not MediaElementState.Playing)
        {
            Play?.Invoke();
        }
    }

    [RelayCommand]
    private void OnPause()
    {
        if (PlaybackState is MediaElementState.Playing)
        {
            Pause?.Invoke();
        }
    }

    [RelayCommand]
    private void OnSeek(TimeSpan position)
    {
        Seek?.Invoke(position);
    }

    [RelayCommand]
    private void MediaOpened()
    {
        if (_project is Project project)
        {
            IntervalLength = project.Interval;
        }
    }

}