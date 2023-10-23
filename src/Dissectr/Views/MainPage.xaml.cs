using CommunityToolkit.Maui.Views;
using System.ComponentModel;

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
}

