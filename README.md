# Automaty

Automaty is a .NET Core based code automation tool for .NET Core projects. It's similiar to and inspired by T4, [Scripty](https://github.com/daveaglick/Scripty) and [dotnet-script](https://github.com/filipw/dotnet-script). What distinguishes Automaty from the the rest? It's the combination of the following points:

  * It supports (only) .NET Core based projects
  * It's implemented as an [.NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/extensibility) tool
  * Code generation is done by plain C# files (.cs), which gives full intellisense support
  * NuGet and project references can be read and resolved from the project file by the [RuntimeLibraryResolver.cs](https://github.com/Dresel/Automaty/blob/master/src/Automaty.Core/Resolution/NuGetPackageResolver.cs)
  * Additional folders and files (within the same project) can be included by attributes or comment directives
  * The MSBuild package automatically runs Code generation on build (either dotnet build or within Visual Studio)

## Getting started

Edit your project file and add the DotNetCli tool as reference to your project (and Automaty.Common if you want to use the `IAutomatyHost`):

    <ItemGroup>
        <PackageReference Include="Automaty.Common" Version="1.0.0-alpha3" />
        <DotNetCliToolReference Include="Automaty.DotNetCli" Version="1.0.0-alpha3" />
    </ItemGroup>

Create a `HelloWorld.cs` class file with an `Execute` function that you want to use for code generation:

    public class HelloWorld
    {
        public void Execute()
        {
            File.WriteAllText(".\\helloworld.txt", "Hello World!");
        }
    }
    
Open a command line and navigate to the project folder. Run

    dotnet automaty run HelloWorld.cs --verbose
    
This should create a `helloworld.txt` within the same directory.

### Using the IAutomatyHost interface

// TODO

### Resolving NuGet and project references

// TODO

### Including additional folders and files

// TODO

### Using the MSBuild task

// TODO
