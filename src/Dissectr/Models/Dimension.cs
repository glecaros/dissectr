using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Dissectr.Models;

public partial class Dimension: ObservableObject
{
    [ObservableProperty]
    private Guid id = default;

    [ObservableProperty]
    private int order = default;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private bool optional = false;

    [ObservableProperty]
    private ObservableCollection<DimensionOption> dimensionOptions = new();

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
