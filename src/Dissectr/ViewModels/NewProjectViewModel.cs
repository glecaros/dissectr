using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        var result = await FilePicker.Default.PickAsync(new()
        {
            FileTypes = FilePickerFileType.Videos,
            PickerTitle = "Select a video",
        });
        FilePath = result switch
        {
            null => string.Empty,
            FileResult fileResult => fileResult.FullPath,
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
        var result = await FilePicker.Default.PickAsync(new()
        {
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".dissectr" } },
                { DevicePlatform.macOS, new[] { "dissectr" } },
            }),
        });
        if (result is null)
        {
            return;
        }
        var imported = await Project.LoadAsync(result.FullPath);
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
        dimension.DimensionOptions.Add(new DimensionOption(Guid.NewGuid(), maxCode + 1, "New option"));
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
        Project project = new(Name, videoFile, IntervalLength, Dimensions.ToList());
        var file = await Project.CreateAsync(path, project);
        await Shell.Current.GoToAsync($"//project?path={file}");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("//start");
    }
}
