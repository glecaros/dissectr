using System.ComponentModel;

using CommunityToolkit.Maui.Views;
using Dissectr.Models;
using Dissectr.Util;

namespace Dissectr.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnUnloaded(object sender, EventArgs e)
    {
        mediaElement.Handler?.DisconnectHandler();
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

    private void DimensionOptionSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var listView = sender as ListView;
        if (listView?.BindingContext is IntervalEntry.DimensionSelection entry)
        {
            entry.Selection = e.SelectedItem as DimensionOption;
            foreach (var o in entry.Dimension.DimensionOptions)
            {
                o.IsSelected = o == entry.Selection;
            }
            entry.IsVisible = listView.SelectedItem is null;
        }
    }

    private void DimensionListLoaded(object sender, EventArgs e)
    {
        var listView = sender as ListView;
        if (listView?.BindingContext is IntervalEntry.DimensionSelection entry)
        {
            listView.SelectedItem = entry.Selection;
            foreach (var o in entry.Dimension.DimensionOptions)
            {
                o.IsSelected = o == entry.Selection;
            }
        }
    }

    private void ToggleDimension(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.BindingContext is IntervalEntry.DimensionSelection entry)
        {
            entry.IsVisible = !entry.IsVisible;
            if (button.Parent.FindByName<ListView>("dimensionList") is ListView list)
            {
                list.IsVisible = entry.IsVisible;
            }
        }

    }
}

