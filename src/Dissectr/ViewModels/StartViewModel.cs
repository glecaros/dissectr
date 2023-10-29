using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Dissectr.ViewModels;

public class StartViewModel: ObservableObject
{
    public ICommand AppearingCommand { get; }
    public ICommand NewProjectCommand { get; }
    public ICommand OpenProjectCommand { get; }

    private AppData appData = new();

    public StartViewModel()
    {
        RecentProjects = new();
        AppearingCommand = new AsyncRelayCommand(AppearingHandler);
        NewProjectCommand = new AsyncRelayCommand(NewProjectHandler);
        OpenProjectCommand = new RelayCommand(OpenProjectHandler);
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
    private async Task NewProjectHandler()
    {
        await Shell.Current.GoToAsync("//new-project");
    }

    private void OpenProjectHandler()
    {
        throw new NotImplementedException();
    }
}
