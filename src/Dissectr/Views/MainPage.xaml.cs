using System.ComponentModel;

using CommunityToolkit.Maui.Views;

using Dissectr.Util;

namespace Dissectr.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    void OnAppearing(object sender, EventArgs e)
    {
        base.OnAppearing();
        var source = MediaSource.FromFile(@"C:\Users\gelecaro\OneDrive\Trabajo Sandra\Cristian.and.Caren_7°C_ClaseN°3y4_11.10.22.mp4");
        mediaElement.Source = source;
    }

    void OnDisappearing(object sender, EventArgs e)
    {
        base.OnDisappearing();
    }

    private void OnUnloaded(object sender, EventArgs e)
    {
        mediaElement.Handler?.DisconnectHandler();
    }

    private void MediaOpened(object sender, EventArgs e)
    {
    }

    private void OnPlay() => mediaElement?.Play();
    private void OnPause() => mediaElement?.Pause();
    private void OnSeek(TimeSpan position) => mediaElement?.SeekTo(position);

    private IMediaControl? currentMediaControl;
    private void OnBindingContextChanged(object sender, EventArgs e)
    {
        if (currentMediaControl is not null)
        {
            currentMediaControl.Play -= OnPlay;
            currentMediaControl.Pause -= OnPause;
            currentMediaControl.Seek -= OnSeek;
        }
        if (BindingContext is IMediaControl mediaControl)
        {
            mediaControl.Play += OnPlay;
            mediaControl.Pause += OnPause;
            mediaControl.Seek += OnSeek;
        }
        currentMediaControl = BindingContext as IMediaControl;
    }
}

