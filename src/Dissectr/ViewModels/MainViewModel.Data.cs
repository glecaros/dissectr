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
    private async Task OnOptionSelected(DimensionOption option)
    {
        if (_project is null)
        {
            throw new ApplicationException("Unexpected error, project is null");
        }
        if (intervalEntry is null)
        {
            throw new ApplicationException("Unexpected error, intervalEntry is null");
        }
        if (Dimensions is null)
        {
            throw new ApplicationException("Unexpected error, dimensions is null");
        }
        var dimension = Dimensions.First(d => d.Dimension.Id == option.DimensionId);
        dimension.Selection = option;
        intervalEntry.Dimensions = Dimensions;
        await _project.SaveEntry(intervalEntry);
    }

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
            intervalEntry.Dimensions = Dimensions ?? new();
            await _project.SaveEntry(entry);
        }
        intervalEntry = await _project.GetEntry(newInterval.Start);
        Transcription = intervalEntry.Transcription;
        Dimensions = new(intervalEntry.Dimensions);
    }
}
