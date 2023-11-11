using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using Dissectr.Views;

namespace Dissectr.ViewModels;

public partial class MainViewModel
{
    IntervalEntry? intervalEntry;

    private ref string? transcription
    {
        get => ref intervalEntry?.Transcription;
        set
        {
            if (intervalEntry is IntervalEntry entry)
            {
                entry.Transcription = value ?? string.Empty;
            }
        }
    }
    public string? Transcription
    {
        get => intervalEntry?.Transcription;
        set => SetProperty(ref transcription, value)

    }

    [RelayCommand]
    private async Task OnIntervalChaging(Interval newInterval)
    {
        if (_project is null)
        {
            throw new ApplicationException("Unexpected error, project is null");
        }
        /* TODO: Move interval out of views */
        if (intervalEntry is IntervalEntry entry)
        {
            //_project.Save(entry);
        }
        intervalEntry = await _project.GetEntry(newInterval.Start);
    }
}
