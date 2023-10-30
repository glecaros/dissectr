namespace Dissectr.Models;

public class Dimension
{
    public required string Name { get; set; }
    public bool Optional { get; set; }
    public required List<DimensionOption> DimensionOptions { get; set; }
}
