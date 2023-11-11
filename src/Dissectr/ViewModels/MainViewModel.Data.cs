using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using Dissectr.Views;
using System.Collections.ObjectModel;

namespace Dissectr.ViewModels;

public partial class MainViewModel
{
    IntervalEntry? intervalEntry;

    [ObservableProperty]
    private string? transcription;

    [ObservableProperty]
    private ObservableCollection<IntervalEntry.DimensionSelection>? dimensions;

    [RelayCommand]
    private async Task OnIntervalChanging(Interval newInterval)
    {
        if (_project is null)
        {
            throw new ApplicationException("Unexpected error, project is null");
        }
        /* TODO: Move interval out of views */
        if (intervalEntry is IntervalEntry entry)
        {
            intervalEntry.Transcription = Transcription ?? string.Empty;
            intervalEntry.Dimensions = Dimensions?.ToList() ?? new();
            await _project.SaveEntry(entry);
        }
        intervalEntry = await _project.GetEntry(newInterval.Start);
        Transcription = intervalEntry.Transcription;
        Dimensions = new(intervalEntry.Dimensions);
    }
}
