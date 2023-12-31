﻿using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using LukeMauiFilePicker;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dissectr.ViewModels;

public partial class NewProjectViewModel: ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(AllowEmptyStrings = false)]
    private string name = string.Empty;

    [ObservableProperty]
    private TimeSpan intervalLength = TimeSpan.FromSeconds(10);

    [ObservableProperty]
    public ObservableCollection<Dimension> dimensions = new();

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(AllowEmptyStrings = false)]
    public string filePath = string.Empty;

    [ObservableProperty]
    public bool isValid = false;

    public NewProjectViewModel()
    {
        ErrorsChanged += ErrorsChangedHandler;
    }

    private void ErrorsChangedHandler(object? sender, DataErrorsChangedEventArgs e)
    {
        IsValid = !HasErrors;
    }

    [RelayCommand]
    private async Task Browse()
    {
        var picker = FilePickerService.Instance;
        var fileTypes = new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new [] { ".mp4",".mpg", ".mpeg", ".mov", ".avi" } },
            { DevicePlatform.macOS, new [] { "mp4", "mpg", "mpeg", "mov", "avi" } },
        };
        var result = await picker.PickFileAsync("Select a video", fileTypes);
        FilePath = result switch
        {
            null => string.Empty,
            IPickFile pickFile => pickFile.FileResult switch
            {
                null => string.Empty,
                FileResult fileResult => fileResult.FullPath,
            },
        };
    }

    [RelayCommand]
    private void IntervalLengthChanged(ValueChangedEventArgs args)
    {
        IntervalLength = TimeSpan.FromSeconds(args.NewValue);
    }

    [RelayCommand]
    private void AddDimension()
    {
        Dimensions.Add(new Dimension
        {
            Id = Guid.NewGuid(),
        });
    }

    [RelayCommand]
    private async Task ImportDimensions()
    {
        var picker = FilePickerService.Instance;
        var fileTypes = new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new[] { ".dissectr" } },
            { DevicePlatform.macOS, new[] { "dissectr" } },
        };
        var result = await picker.PickFileAsync("Select an existing project", fileTypes);
        string? projectPath = result?.FileResult?.FullPath;
        if (projectPath is null)
        {
            return;
        }
        var imported = await Project.LoadAsync(projectPath);
        foreach (var dimension in imported.Dimensions)
        {
            Dimensions.Add(dimension);
        }
    }

    [RelayCommand]
    private void RemoveDimension(Dimension? dimension)
    {
        if (dimension is null)
        {
            return;
        }
        Dimensions.Remove(dimension);
    }

    [RelayCommand]
    private void AddDimensionOption(Dimension? dimension)
    {
        if (dimension is null)
        {
            return;
        }
        var maxCode = dimension.DimensionOptions.Count switch
        {
            0 => 0,
            _ => dimension.DimensionOptions.Select(d => d.Code).Max(),
        };
        dimension.DimensionOptions.Add(new() { Id = Guid.NewGuid(), DimensionId = dimension.Id, Code = maxCode + 1, Name = "New option" });
    }

    [RelayCommand]
    private void RemoveDimensionOption(DimensionOption? dimensionOption)
    {
        if (dimensionOption is null)
        {
            return;
        }
        var dimension = Dimensions.FirstOrDefault(d => d.DimensionOptions.Contains(dimensionOption));
        if (dimension is null)
        {
            return;
        }
        dimension.DimensionOptions.Remove(dimensionOption);
    }

    [RelayCommand(CanExecute = nameof(IsValid))]
    private async Task Save()
    {
        var path = Path.GetDirectoryName(FilePath);
        var videoFile = Path.GetFileName(FilePath);
        var fullProjectPath = Path.ChangeExtension(Path.Combine(path, Name), ".dissectr");
        Project project = new(Name, fullProjectPath, videoFile, IntervalLength, Dimensions.ToList());
        var file = await Project.CreateAsync(project);
        await Shell.Current.GoToAsync($"//main?path={file}");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("//start");
    }
}
