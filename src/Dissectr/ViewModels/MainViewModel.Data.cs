using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using Dissectr.Views;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

    [RelayCommand]
    private void ToggleDimensionVisibility(Guid dimensionId)
    {
        var dimension = Dimensions?.FirstOrDefault(d => d.Dimension.Id == dimensionId);
        if (dimension is not null)
        {
            dimension.IsVisible = !dimension.IsVisible;
        }
    }

    [RelayCommand]
    private async Task ExportToXLS()
    {
        var result = await FilePicker.Default.PickAsync(new()
        {
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".xlsx" } },
                { DevicePlatform.macOS, new[] { "xlsx" } },
            }),
        });
        if (result is null)
        {
            return;
        }
        if (Path.Exists(result.FileName))
        {
            bool answer = await DisplayAlert("Confirmation", "That file already exists, do you want to overwrite it?", "Yes", "No");
        }
    }
}
