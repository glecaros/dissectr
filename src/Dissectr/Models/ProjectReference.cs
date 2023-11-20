using System;

namespace Dissectr.Models;

public class ProjectReference
{
    public Guid Id { get; set; }
    public DateTimeOffset LastOpened { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}
