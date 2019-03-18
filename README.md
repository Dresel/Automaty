# Automaty

| Win (development)           | Linux (development)  |
| :-------------: | :-------------: |
| [![Build status](https://dev.azure.com/dresel/Automaty/_apis/build/status/Automaty%20CI%20(win)?branchName=development)](https://dev.azure.com/dresel/Automaty/_build/latest?definitionId=15) | [![Build status](https://dev.azure.com/dresel/Automaty/_apis/build/status/Automaty%20CI%20(linux)?branchName=development)](https://dev.azure.com/dresel/Automaty/_build/latest?definitionId=14) |

Automaty is a .NET Core based code automation tool for .NET Core projects. It's similiar to and inspired by T4, [Scripty](https://github.com/daveaglick/Scripty) and [dotnet-script](https://github.com/filipw/dotnet-script). What distinguishes Automaty from the the rest? It's the combination of the following points:

* It supports (only) .NET Core based projects
* It's implemented as an [.NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/extensibility) tool
* Code generation is done by plain C# files (.cs), which gives full intellisense support
* NuGet and project references can be read and resolved from the project file by the [RuntimeLibraryResolver.cs](https://github.com/Dresel/Automaty/blob/master/src/Automaty.Core/Resolution/RuntimeLibraryResolver.cs)
* Additional folders and files (within the same project) can be included by attributes or comment directives
* The MSBuild package automatically runs Code generation on build (either dotnet build or within Visual Studio)

Criticism, comments and or suggestions are welcome.

## Automaty based generators

* [EFCoreRepositories](https://github.com/Dresel/Automaty.Generators.EFCoreRepositories) An Automaty based generator for Entity Framework Core repositories. Creates strongly typed repositories based on the IModel of your context. 

## Getting started

Edit your project file and add the DotNetCli tool as reference to your project (and Automaty.Common if you want to use the `IAutomatyHost` interface):

```xml
<ItemGroup>
    <PackageReference Include="Automaty.Common" Version="1.0.1-*" />
    <DotNetCliToolReference Include="Automaty.DotNetCli" Version="1.0.1-*" />
</ItemGroup>
```

Create a `HelloWorld.cs` class file with an `Execute` function that you want to use for code generation:

```csharp
public class HelloWorld
{
    public void Execute()
    {
        File.WriteAllText("helloworld.txt", "Hello World!");
    }
}
```
    
Open a command line and navigate to the project folder. Run

```bash
dotnet automaty run HelloWorld.cs --verbose
```

This should create a `helloworld.txt` within the same directory.

### Using the IAutomatyHost interface

The above sample shows everything you need for simple scripts. For more complex tasks you could use the `IAutomatyHost` interface:

```csharp
    public void Execute(IScriptContext context)
    {
        context.Output["helloworld.txt"].WriteLine("Hello World!");
    }
```

The interface contains properties and functions that might be useful for code generation.

### Resolving NuGet and project references

Automaty reads the project file for NuGet and project references and adds them to the underlying compilation. 
This makes it possible to use libraries like EntityFrameworkCore for code generation. The resolution of NuGet packages is based on the [NuGet lock file](https://stackoverflow.com/questions/38065611/what-is-project-lock-json), so be sure to call `dotnet restore` before calling Automaty.

```xml
<ItemGroup>
    <PackageReference Include="Automaty.Common" Version="1.0.1-*" />
    <DotNetCliToolReference Include="Automaty.DotNetCli" Version="1.0.1-*" />

    <PackageReference Include="Newtonsoft.Json" Version="9.0.1" />
</ItemGroup>
```

```csharp
public class HelloWorld : IAutomatyHost
{
    public void Execute(IScriptContext context)
    {
        context.Output["helloworld.json"].WriteLine(JsonConvert.SerializeObject("Hello World!"));
    }
}
```

Don't forget to pass the project file as additional parameter:

```bash
dotnet automaty run HelloWorld.cs --project HelloWorld.csproj --verbose
```

### Including additional folders and files

Automaty does only compile the given class file. If you want to use additional files or folders in the same solution, you have to define them by attributes or comment directives:

```csharp
// Automaty IncludeFiles ./File1.cs;./../File2.cs
[AutomatyIncludeDirectory(Directory = "./Data")]
public class Repository : IAutomatyHost
{
    // ...
}
```

Supported comment directives:

* // IncludeFile filepath
* // IncludeFiles filepath1;filepath2[;filepath]
* // IncludeDirectory directory
* // IncludeDirectories directory1;directory2[;directory]

Supported attributes:

* [AutomatyIncludeFilesAttribute.cs](https://github.com/Dresel/Automaty/blob/master/src/Automaty.Common/Execution/AutomatyIncludeFileAttribute.cs)
* [AutomatyIncludeFilesAttributes.cs](https://github.com/Dresel/Automaty/blob/master/src/Automaty.Common/Execution/AutomatyIncludeFilesAttribute.cs)
* [AutomatyIncludeDirectoryAttribute.cs](https://github.com/Dresel/Automaty/blob/master/src/Automaty.Common/Execution/AutomatyIncludeDirectoryAttribute.cs)
* [AutomatyIncludeDirectoriesAttribute.cs](https://github.com/Dresel/Automaty/blob/master/src/Automaty.Common/Execution/AutomatyIncludeDirectoriesAttribute.cs)

### Using the MSBuild task

If you do not want to use the DotNetCli by hand, you can use the MSBuild task:

```xml
<ItemGroup>
    <PackageReference Include="Automaty.MSBuild" Version="1.0.1-*" />
    <DotNetCliToolReference Include="Automaty.DotNetCli" Version="1.0.1-*" />
</ItemGroup>
```

By default, every file that ends with `.Automaty.cs` like `HelloWorld.Automaty.cs` will be processed by the task. You can overwrite this by specifying `<AutomatyFile>`:

```xml
<ItemGroup>
    <AutomatyFile Include="HelloWorld.cs" />
</ItemGroup>
```

Usually you won't see any logs by Automaty while building (except warnings and errors), to enable that you have to modify the build verbosity:

**Visual Studio**

Go to `Tools / Options / Project and Solutions / Build and Run` and set MSBuild verbosity to `Normal` or above.

**dotnet build**

Run `dotnet build` with the `-v` parameter set to `n[ormal]` or above.

To enable verbose output for the MSBuild task, set

```xml
<PropertyGroup>
    <AutomatyIsVerbose>True</AutomatyIsVerbose>
</PropertyGroup>
```

### Miscellaneous

If you like the nested file structure used by `T4` templates, you could add something like this to your project file:

```xml
<ItemGroup>
    <Compile Update="HelloWorld.*.cs">
        <DependentUpon>HelloWorld.cs</DependentUpon>
    </Compile>
</ItemGroup>
```
