using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using Dissectr.Services;
using Microsoft.Maui.Controls;

namespace Dissectr.ViewModels;

[QueryProperty(nameof(ProjectPath), "path")]
public partial class MainViewModel
{
    private readonly IAlertService _alertService;

    public MainViewModel()
    {
        _alertService = new AlertService();
    }

    [ObservableProperty]
    MediaSource? mediaSource;

    public string ProjectPath
    {
        set
        {
            Task.Run(async () => await LoadProject(value));
        }
    }

    private Project? _project;

    private async Task LoadProject(string path)
    {
        _project = await Project.LoadAsync(path);
        if (_project is null)
        {
            throw new ArgumentNullException("Loading project failed");
        }
        var videoPath = Path.Combine(Path.GetDirectoryName(path), _project.VideoFile);
        MediaSource = MediaSource.FromFile(videoPath);
    }


}
