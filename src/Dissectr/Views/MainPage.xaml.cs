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
    }

    void OnDisappearing(object sender, System.EventArgs e)
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

    private void OnBindingContextChanged(object sender, EventArgs e)
    {
        if (BindingContext is IMediaControl mediaControl)
        {
            mediaControl.Play += OnPlay;
            mediaControl.Pause += OnPause;
            mediaControl.Seek += OnSeek;
        }

    }
}

