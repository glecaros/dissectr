using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dissectr.ViewModels;


public class ProjectViewModel: ObservableObject
{
    public ICommand AddVideoFile { get; }

    public ObservableCollection<string> Files { get; } = new();

    public ProjectViewModel()
    {
        AddVideoFile = new AsyncRelayCommand(AddVideoFileHandler);
    }

    private async Task AddVideoFileHandler()
    {
        //var result = await FilePicker.PickAsync(new PickOptions
        //{
        //    PickerTitle = "Select a video file",
        //    FileTypes = FilePickerFileType.,
        //});

        //if (result is FileResult fileResult)
        //{

        //    Files.Add(fileResult.FullPath);
        //}
    }
}
