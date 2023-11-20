using Microsoft.Maui.Controls;

namespace Dissectr;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
