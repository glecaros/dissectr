using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Dissectr.Models;

public partial class Dimension: ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private bool optional = false;

    [ObservableProperty]
    private ObservableCollection<DimensionOption> dimensionOptions = new();

}
