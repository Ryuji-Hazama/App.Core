using App.Core.Containers;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace App.Core.Components;

#region ComponentsLibrary Class

/// <summary>
/// Represents a library of components that can be dynamically loaded and instantiated from assemblies.
/// </summary>
public class ComponentsLibrary : IComponentsLibrary
{
    #region Class Members

    private readonly Config Config = App.ConfigManager.GetConfig();
    private readonly ComponentsJson Components_Json;
    private readonly List<ComponentDictionary> Components = new();

    #endregion Class Members

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentsLibrary"/> class.
    /// Loads the components list from the configuration and sets up assembly resolution.
    /// </summary>
    public ComponentsLibrary()
    {
        AssemblyLoadContext.Default.Resolving += ResolveAssembly;

        string componentsListPath = Config.App.ComponentsList;

        if (string.IsNullOrWhiteSpace(componentsListPath))
            Components_Json = new ComponentsJson();
        else if (File.Exists(componentsListPath))
        {
            string json = File.ReadAllText(componentsListPath);
            Components_Json = JsonSerializer.Deserialize<ComponentsJson>(json) ?? new ComponentsJson();
        }
        else
            throw new FileNotFoundException($"Components list file not found: {componentsListPath}");
    }

    #endregion Constructor

    #region Public Methods

    /// <summary>
    /// Invokes the specified component from the given assembly and context with optional parameters.
    /// </summary>
    /// <param name="assembly_path">The path to the assembly containing the component.</param>
    /// <param name="context">The fully qualified name of the component type.</param>
    /// <param name="parameters">Optional parameters to pass to the component's constructor.</param>
    /// <returns>The instantiated component object.</returns>
    /// <exception cref="TypeLoadException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public object Invoke(string assembly_path, string context, object[]? parameters)
    {
        var assembly = AssemblyLoadContext.Default.Assemblies
            .FirstOrDefault(existing => existing.GetName().Name == Path.GetFileNameWithoutExtension(assembly_path))
            ?? AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(assembly_path));
        var type = assembly.GetType(context);
        if (type is null)
        {
            throw new TypeLoadException($"Type '{context}' not found in assembly '{assembly_path}'.");
        }

        // Allow binding to constructors with trailing optional parameters not covered by 'parameters'.
        var instance = Activator.CreateInstance(
            type,
            BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding,
            null,
            parameters ?? Array.Empty<object>(),
            null);
        if (instance is null)
        {
            throw new InvalidOperationException($"Could not create an instance of type '{context}'.");
        }

        return instance;
    }

    /// <summary>
    /// Retrieves a component by its name, creating and caching it if necessary.
    /// </summary>
    /// <typeparam name="T">The expected type of the component.</typeparam>
    /// <param name="component_name">The name of the component to retrieve.</param>
    /// <param name="parameters">Optional parameters to pass to the component's constructor if it needs to be created.</param>
    /// <param name="use_interface">Whether to use the component's interface or context when creating it.</param>
    /// <returns>The requested component cast to the specified type.</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="TypeLoadException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public T GetComponent<T>(string component_name, object[]? parameters = null)
    {
        // Check if an existing component with the same name and parameters already exists
        var existing_component = FindExistingComponent(component_name, parameters);
        if (existing_component != null)
            return (T)existing_component;

        // If no existing component matches, create a new one
        Component component_info = Components_Json.Components.FirstOrDefault(c => c.Name == component_name)
            ?? throw new KeyNotFoundException($"Component '{component_name}' not found.");
        string assembly_path = Path.IsPathFullyQualified(component_info.Assembly) ? component_info.Assembly
            : Path.Combine(Config.App.AssemblyPath, component_info.Assembly);
        var new_component = Invoke(assembly_path, component_info.Context, parameters);

        // Add the newly created component to the list of existing components
        ComponentDictionary component_dict = new ComponentDictionary
        {
            Name = component_name,
            Parameters = parameters ?? Array.Empty<object>(),
            Component = new_component
        };
        Components.Add(component_dict);
        return (T)new_component;
    }

    #endregion Public Methods

    #region Private Methods

    private Assembly? ResolveAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        string dependencyPath = Path.GetFullPath(
            Path.Combine(Config.App.AssemblyPath, assemblyName.Name + ".dll"));
        return File.Exists(dependencyPath) ? context.LoadFromAssemblyPath(dependencyPath) : null;
    }

    private object? FindExistingComponent(string component_name, object[]? parameters = null)
    {
        // Look for existing components with the specified name
        var component = Components.Where(c => c.Name == component_name);
        if (component.Count() > 0)
        {
            // Check if any existing component matches the requested parameters
            foreach (var c in component)
            {
                if (c.Parameters.Count() == 0 && (parameters is null || parameters.Length == 0))
                    return c.Component;

                if (c.Parameters.Length == parameters?.Length)
                {
                    bool match = true;
                    for (int i = 0; i < c.Parameters.Length; i++)
                    {
                        if (parameters[i] is null && c.Parameters[i] is null)
                            continue;   // Avoid null comparison issues
                        else if (!c.Parameters[i].Equals(parameters[i]))
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                        return c.Component;
                }
            }
        }

        return null;
    }

    #endregion Private Methods
}

#endregion ComponentsLibrary Class

#region IComponentsLibrary Interface

/// <summary>
/// Interface for accessing components from the components library.
/// </summary>
public interface IComponentsLibrary
{
    T GetComponent<T>(string component_name, object[]? parameters = null);
}

#endregion IComponentsLibrary Interface