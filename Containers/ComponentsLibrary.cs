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
}

public class ComponentDictionary
{
    public string Name { get; set; } = string.Empty;
    public object[] Parameters { get; set; } = Array.Empty<object>();
    public object Component { get; set; } = new object();
}