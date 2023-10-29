using Dissectr.Views;

namespace Dissectr;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("new-project", typeof(NewProjectPage));
    }
}
