using CommunityToolkit.Maui.Views;
using System.ComponentModel;

namespace Dissectr.Views;

/// <summary>
/// Represents the main viewmodel.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Property changed event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Initializes a new instance of <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel()
    {
    }


    /// <summary>
    /// </summary>
    public void OnAppearing()
    {
    }

    internal void OnDisappearing()
    {
    }

    private void Set<T>(string propertyName, ref T field, T value)
    {
        if (field == null && value != null || field != null && !field.Equals(value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


public partial class MainPage : ContentPage
{
    private TimeSpan Position { get; set; }
    private TimeSpan Duration { get; set; }

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
}

