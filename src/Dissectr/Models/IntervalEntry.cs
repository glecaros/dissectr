using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace Dissectr.Models;

public partial class IntervalEntry: ObservableObject
{

    public partial class DimensionSelection: ObservableObject
    {
        public DimensionSelection(Dimension dimension, DimensionOption? selection = null)
        {
            Dimension = dimension;
            Selection = selection;
        }

        [ObservableProperty]
        private Dimension dimension;

        [ObservableProperty]
        private DimensionOption? selection;

        [ObservableProperty]
        private bool isVisible = true;
    }

    public required TimeSpan Start { get; init; }

    [ObservableProperty]
    public string? transcription;

    [ObservableProperty]
    public ObservableCollection<DimensionSelection> dimensions = new();
}
