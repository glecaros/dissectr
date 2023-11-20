using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Dissectr.Models;

public partial class DimensionOption : ObservableObject
{
    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private Guid dimensionId;

    [ObservableProperty]
    private int code;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private bool isSelected = false;

    public override bool Equals(object? obj)
    {
        if (obj is DimensionOption other)
        {
            return Id == other.Id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
