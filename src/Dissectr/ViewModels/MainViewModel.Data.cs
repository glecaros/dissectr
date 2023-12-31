﻿using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using Dissectr.Util;
using Dissectr.Views;
using LukeMauiFilePicker;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
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
    private async Task ExportToXLS(CancellationToken cancellationToken = default)
    {
        if (_project is null)
        {
            throw new ApplicationException("Unexpected error, project is null");
        }
        var memoryStream = new MemoryStream();
        var entries = await _project.GetEntries();
        Exporter.ExportToXLS(_project, entries, memoryStream);

        var picker = FilePickerService.Instance;
        var success = await picker.SaveFileAsync(new($"{_project.Name}.xlsx", memoryStream)
        {
            WindowsFileTypes = ("XLSX File", new() { ".xlsx" }),
        });
        if (success)
        {
            await _alertService.ShowAlertAsync("Notice", "Success!");
        }
        else
        {
            await _alertService.ShowAlertAsync("Alert", "Issue saving file");
        }
    }
}
