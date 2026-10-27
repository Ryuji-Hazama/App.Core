# Components Library

The Components Library provides a way to manage and access components within the application. It allows you to retrieve components by name and optionally pass parameters to their constructors. The library ensures that existing components are reused when possible, avoiding unnecessary instantiations.

## Usage

To get a component from the library, use the `GetComponent` method:

```csharp
MyComponentType myComponent = componentsLibrary.GetComponent<MyComponentType>("ComponentName", new object[] { param1, param2 });
```

- `MyComponentType`: The type of the component you want to retrieve.
- `"ComponentName"`: The name of the component as defined in the components JSON.
- `new object[] { param1, param2 }`: Optional parameters to pass to the component's constructor.

The library will first check if an existing component with the same name and parameters exists. If found, it will return the existing instance. Otherwise, it will create a new instance and add it to the list of existing components.

### Use Instances

When you work with other project team or outside collaborators, you can receive and use only its instances through the Components Library. This ensures that the components are properly managed and reused, avoiding unnecessary instantiations and maintaining consistency across the application.

```csharp
ICollabLibrary collabInstance = componentsLibrary.GetComponent<ICollabLibrary>("CollabLibraryComponent", new object[] { param1, param2 });
```

## Tips

### Project File

You can automate copying the necessary component assemblies to the output directory by configuring your project file. This ensures that all required components are available at runtime without manual intervention.

In this README, we will use `Assembly` directory for gather all component assemblies that need to be copied to the output directory, and read them from `<ApplicationRoot>/AppData/bin` at runtime.

#### Collaborator Project File Configuration

To copy the built component assembly to the output directory after buildding, you can add the following configuration to your project file (`.csproj`):

```xml
<!-- Copy built DLL to Assembly directory -->
<Target Name="CopyComponentAssemblies" AfterTargets="Build">
    <Copy
        SourceFiles="$(OutputPath)CollabLibrary.dll"
        DestinationFolder="../Assembly/AppData/bin"
        SkipUnchangedFiles="true"
        Condition="Exists('$(OutputPath)CollabLibrary.dll')"
    />
</Target>
```

#### Main Project File Configuration

To copy all component assemblies from the `Assembly` directory to the output directory after building, you can add the following configuration to your main project file (`.csproj`):

```xml
<!-- Copy component assemblies to output directory -->
<Target Name="CopyComponentAssemblies" AfterTargets="Build">
    <Copy
        SourceFiles="../Assembly/AppData/bin/*.dll"
        DestinationFolder="$(OutputPath)/AppData/bin"
        SkipUnchangedFiles="true"
        Condition="Exists('../Assembly/AppData/bin')"
    />
</Target>
```

or simply:

```xml
<ItemGroup>
    <None Update="../Assembly/AppData/bin/*.dll">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
</ItemGroup>
```
