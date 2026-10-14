namespace Lune.Api.Models;

public class BehaviorType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}