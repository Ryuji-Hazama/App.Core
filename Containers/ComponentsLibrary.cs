namespace App.Core.Containers;

public class ComponentsJson
{
    public List<Component> Components { get; set; } = new List<Component>();
}

public class Component
{
    public string Name { get; set; } = string.Empty;
    public string Assembly { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string Interface { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
