using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace App.Core.Components
{
    internal class ComponentsJson
    {
        public List<Component> Components { get; set; } = new List<Component>();
    }

    internal class Component
    {
        public string Name { get; set; } = string.Empty;
        public string Assembly { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public string Interface { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class ComponentsLibrary : IComponentsLibrary
    {
        private readonly App.Config _config = App.ConfigManager.GetConfig();
        private readonly ComponentsJson Components_Json = new ComponentsJson();
        private readonly Dictionary<string, object> _components = new();

        public ComponentsLibrary()
        {
            AssemblyLoadContext.Default.Resolving += ResolveAssembly;

            string componentsListPath = _config.App.ComponentsList;

            if (File.Exists(componentsListPath))
            {
                string json = File.ReadAllText(componentsListPath);
                Components_Json = JsonSerializer.Deserialize<ComponentsJson>(json) ?? new ComponentsJson();
            }
            else
            {
                throw new FileNotFoundException($"Components list file not found: {componentsListPath}");
            }
        }

        private Assembly? ResolveAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            string dependencyPath = Path.GetFullPath(
                Path.Combine(_config.App.AssemblyPath, assemblyName.Name + ".dll"));
            return File.Exists(dependencyPath) ? context.LoadFromAssemblyPath(dependencyPath) : null;
        }

        public object Invoke(string assemblyName, string context, object[]? parameters)
        {
            string assemblyPath = Path.Combine(_config.App.AssemblyPath, assemblyName);

            if (!File.Exists(assemblyPath))
            {
                throw new FileNotFoundException($"Assembly file not found: {assemblyPath}");
            }

            var assembly = AssemblyLoadContext.Default.Assemblies
                .FirstOrDefault(existing => existing.GetName().Name == Path.GetFileNameWithoutExtension(assemblyName))
                ?? AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(assemblyPath));
            var type = assembly.GetType(context);
            if (type == null)
            {
                throw new TypeLoadException($"Type '{context}' not found in assembly '{assemblyName}'.");
            }

            // Allow binding to constructors with trailing optional parameters not covered by 'parameters'.
            var instance = Activator.CreateInstance(
                type,
                BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding,
                null,
                parameters ?? Array.Empty<object>(),
                null);
            if (instance == null)
            {
                throw new InvalidOperationException($"Could not create an instance of type '{context}'.");
            }

            return instance;
        }

        public T GetComponent<T>(string componentName, object[]? parameters = null)
        {
            if (_components.TryGetValue(componentName, out var component))
            {
                return (T)component;
            }else
            {
                Component component_info = Components_Json.Components.FirstOrDefault(c => c.Name == componentName)
                    ?? throw new KeyNotFoundException($"Component '{componentName}' not found.");
                var new_component = Invoke(component_info.Assembly, component_info.Context, parameters);
                _components[componentName] = new_component;
                return (T)new_component;
            }
        }
    }

    public interface IComponentsLibrary
    {
        T GetComponent<T>(string componentName, object[]? parameters = null);
    }
}