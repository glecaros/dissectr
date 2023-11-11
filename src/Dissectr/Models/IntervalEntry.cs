using CommunityToolkit.Mvvm.ComponentModel;

namespace Dissectr.Models;

public class IntervalEntry: ObservableObject
{
    public struct DimensionSelection
    {
        public DimensionSelection(Dimension dimension, DimensionOption? selection = null)
        {
            Dimension = dimension;
            Selection = selection;
        }

        public Dimension Dimension { get; set; }

        public DimensionOption? Selection { get; set; }
    }

    public required TimeSpan Start { get; init; }
    public string Transcription { get; set; } = string.Empty;
    public List<DimensionSelection> Dimensions { get; set; } = new();
}
