using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using System.Windows.Input;

namespace Dissectr.ViewModels;

public class StartViewModel: ObservableObject
{
    public ICommand AppearingCommand { get; }
    public ICommand DisappearingCommand { get; }

    private AppData appData = new();

    public StartViewModel()
    {
        AppearingCommand = new AsyncRelayCommand(AppearingHandler);
        DisappearingCommand = new AsyncRelayCommand(DisappearingHandler);
    }

    public async Task AppearingHandler()
    {
        await appData.LoadAsync();
    }

    public async Task DisappearingHandler()
    {
        await appData.SaveAsync();
    }
}
