using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace Dissectr.ViewModels;

internal partial class MainViewModel: ObservableObject
{
    public TimeSpan Position { get; set; }
    public TimeSpan Duration { get; set; }

    public bool IsPlaying { get; set; }

    public ICommand Play { get; }
    public ICommand Pause { get; }
    public ICommand Stop { get; }
    public ICommand Seek { get; }

    public MainViewModel()
    {
        Play = new AsyncRelayCommand(PlayHandler);
        Pause = new AsyncRelayCommand(PauseHandler);
        Stop = new AsyncRelayCommand(StopHandler);
        Seek = new AsyncRelayCommand<TimeSpan>(SeekHandler);
    }

    private void RefreshProperties()
    {
        OnPropertyChanged(nameof(Position));
        OnPropertyChanged(nameof(Duration));
        OnPropertyChanged(nameof(IsPlaying));
    }

    private async Task PlayHandler()
    {
        if (!IsPlaying)
        {

        }
    }

    private async Task PauseHandler()
    {
        if (IsPlaying)
        {

        }

    }

    private async Task StopHandler()
    {


    }

    private async Task SeekHandler(TimeSpan position) { }


}