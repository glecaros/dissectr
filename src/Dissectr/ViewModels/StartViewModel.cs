using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Dissectr.ViewModels;

public partial class StartViewModel: ObservableObject
{
    public ICommand AppearingCommand { get; }

    private AppData appData = new();

    public StartViewModel()
    {
        RecentProjects = new();
        AppearingCommand = new AsyncRelayCommand(AppearingHandler);
    }

    public ObservableCollection<ProjectReference>  RecentProjects { get; }

    public async Task AppearingHandler()
    {
        var recentProjects = await appData.GetRecentProjectsAsync();
        foreach (var project in recentProjects)
        {
            RecentProjects.Add(project);
        }
    }


    [RelayCommand]
    private async Task NewProject()
    {
        await Shell.Current.GoToAsync("//new-project");
    }

    [RelayCommand]
    private async Task OpenProject()
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
        await Shell.Current.GoToAsync($"//main?path={result.FullPath}");
    }
}
