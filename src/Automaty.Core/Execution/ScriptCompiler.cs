namespace Automaty.Core.Execution
{
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Automaty.Common.Execution;
	using Automaty.Common.Logging;
	using Automaty.Core.Logging;
	using Automaty.Core.Resolution;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Scripting;
	using Microsoft.CodeAnalysis.Text;

	public class ScriptCompiler
	{
		public ScriptCompiler() : this(new NullLogger<ScriptCompiler>())
		{
		}

		public ScriptCompiler(ILogger<ScriptCompiler> logger)
		{
			Logger = logger;

			MetadataReferences = new List<MetadataReference>(ScriptOptions.Default.MetadataReferences)
			{
				// Add Automaty.Core by default
				MetadataReference.CreateFromFile(GetType().GetTypeInfo().Assembly.Location)
			};
		}

		public ILogger<ScriptCompiler> Logger { get; set; }

		public ICollection<MetadataReference> MetadataReferences { get; set; }

		public void AddRuntimeLibraries(IEnumerable<RuntimeLibrary> runtimeLibraries)
		{
			foreach (RuntimeLibrary runtimeLibrary in runtimeLibraries)
			{
				MetadataReferences.Add(MetadataReference.CreateFromFile(runtimeLibrary.FilePath));
			}
		}

		public Compilation Compile(string filePath)
		{
			Logger.WriteInfo("Resolving references.");

			MetadataReferenceResolver metadataReferenceResolver = ScriptOptions.Default.MetadataResolver;
			IEnumerable<MetadataReference> references = ResolveReferences(metadataReferenceResolver);

			string directoryPath = Path.GetDirectoryName(filePath);
			SyntaxTree syntaxTree = CreateSyntaxTreeFromFilePath(filePath);

			ICollection<string> additionalFilePaths = GetAdditionalFilePaths(directoryPath, syntaxTree);

			CSharpCompilation compilation = CreateCompilation(syntaxTree, additionalFilePaths.Except(new[] { filePath }),
				references, metadataReferenceResolver);

			ICollection<string> additionalFilePathsByAttributes =
				GetAdditionalFilePathsByAttributes(directoryPath, compilation.GetSemanticModel(syntaxTree));

			if (additionalFilePathsByAttributes.Any())
			{
				compilation = compilation.AddSyntaxTrees(additionalFilePathsByAttributes.Except(new[] { filePath })
					.Select(CreateSyntaxTreeFromFilePath));
			}

			return compilation;
		}

		protected void AddAdditionalFiles(IEnumerable<string> additionalFiles, string baseDirectoryPath,
			ICollection<string> files)
		{
			foreach (string additionalFile in additionalFiles)
			{
				string additionalFilePath = GetFullPath(baseDirectoryPath, additionalFile);

				Logger.WriteDebug($"Adding file \"{additionalFilePath}\".");

				files.Add(additionalFilePath);
			}
		}

		protected void AddAdditionalFilesAndFolders(string directoryPath, IEnumerable<string> additionalFiles,
			IEnumerable<string> additionalDirectories, ICollection<string> files)
		{
			AddAdditionalFiles(additionalFiles, directoryPath, files);

			foreach (string additionalDirectory in additionalDirectories)
			{
				Logger.WriteDebug($"Adding folder \"{additionalDirectory}\".");
				AddAdditionalFiles(GetFilePathsFromDirectoryPath(directoryPath, additionalDirectory), directoryPath, files);
			}
		}

		protected CSharpCompilation CreateCompilation(SyntaxTree syntaxTree, IEnumerable<string> additionalFilePaths,
			IEnumerable<MetadataReference> references, MetadataReferenceResolver metadataReferenceResolver)
		{
			Logger.WriteInfo("Creating compilation.");

			List<SyntaxTree> syntaxTrees = new List<SyntaxTree>
			{
				syntaxTree
			};

			syntaxTrees.AddRange(additionalFilePaths.Select(CreateSyntaxTreeFromFilePath));

			CSharpCompilation compilation = CSharpCompilation.Create(Path.GetRandomFileName(), syntaxTrees, references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
					metadataReferenceResolver: metadataReferenceResolver));

			return compilation;
		}

		protected SyntaxTree CreateSyntaxTreeFromFilePath(string filePath)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				SourceText sourceText = SourceText.From(fileStream);
				return CSharpSyntaxTree.ParseText(sourceText, path: filePath);
			}
		}

		protected ICollection<string> GetAdditionalFilePaths(string directoryPath, SyntaxTree syntaxTree)
		{
			ICollection<string> additionalFilePaths = new List<string>();

			Logger.WriteDebug("Adding additional files and directories by comment directives.");

			(ICollection<string> files, ICollection<string> directories) additionalFilesAndDirectoriesByCommentDirectives =
				GetAdditionalFilesByCommentDirectives(syntaxTree);
			AddAdditionalFilesAndFolders(directoryPath, additionalFilesAndDirectoriesByCommentDirectives.files,
				additionalFilesAndDirectoriesByCommentDirectives.directories, additionalFilePaths);

			return additionalFilePaths;
		}

		protected ICollection<string> GetAdditionalFilePathsByAttributes(string directoryPath, SemanticModel semanticModel)
		{
			ICollection<string> additionalFilePathsByAttributes = new List<string>();

			Logger.WriteDebug("Adding additional files and directories by attributes.");

			(ICollection<string> files, ICollection<string> directories) additionalFilesAndDirectoriesByAttributes =
				GetAdditionalFilesAndDirectoriesByAttributes(semanticModel);

			AddAdditionalFilesAndFolders(directoryPath, additionalFilesAndDirectoriesByAttributes.files,
				additionalFilesAndDirectoriesByAttributes.directories, additionalFilePathsByAttributes);

			return additionalFilePathsByAttributes;
		}

		protected (ICollection<string> files, ICollection<string> folders) GetAdditionalFilesAndDirectoriesByAttributes(
			SemanticModel semanticModel)
		{
			ICollection<string> additionalFiles = new List<string>();
			ICollection<string> additionalDirectories = new List<string>();

			IEnumerable<AttributeData> attributes = semanticModel.SyntaxTree.GetRoot()
				.DescendantNodes()
				.OfType<ClassDeclarationSyntax>()
				.SelectMany(x => semanticModel.GetDeclaredSymbol(x).GetAttributes());

			foreach (AttributeData attribute in attributes)
			{
				string displayString = attribute.AttributeClass.ToDisplayString();

				if (displayString == typeof(AutomatyIncludeFileAttribute).FullName)
				{
					if (attribute.NamedArguments.Length != 1)
					{
						continue;
					}

					additionalFiles.Add((string)attribute.NamedArguments.Single().Value.Value);
				}
				else if (displayString == typeof(AutomatyIncludeFilesAttribute).FullName)
				{
					if (attribute.NamedArguments.Length != 1)
					{
						continue;
					}

					string[] additionalFilePaths = (string[])attribute.NamedArguments.Single().Value.Value;

					foreach (string additionalFilePath in additionalFilePaths)
					{
						additionalFiles.Add(additionalFilePath);
					}
				}
				else if (displayString == typeof(AutomatyIncludeDirectoryAttribute).FullName)
				{
					if (attribute.NamedArguments.Length != 1)
					{
						continue;
					}

					additionalDirectories.Add((string)attribute.NamedArguments.Single().Value.Value);
				}
				else if (displayString == typeof(AutomatyIncludeDirectoriesAttribute).FullName)
				{
					if (attribute.NamedArguments.Length != 1)
					{
						continue;
					}

					string[] additionalDirectoryPaths = (string[])attribute.NamedArguments.Single().Value.Value;

					foreach (string additionalDirectoryPath in additionalDirectoryPaths)
					{
						additionalDirectories.Add(additionalDirectoryPath);
					}
				}
			}

			return (additionalFiles, additionalDirectories);
		}

		protected (ICollection<string> files, ICollection<string> folders) GetAdditionalFilesByCommentDirectives(
			SyntaxTree syntaxTree)
		{
			ICollection<string> additionalFiles = new List<string>();
			ICollection<string> additionalDirectories = new List<string>();

			SyntaxNode syntaxNode = syntaxTree.GetRoot();

			List<string> commentDirectives = syntaxNode.DescendantTrivia()
				.Where(x => x.IsKind(SyntaxKind.SingleLineCommentTrivia))
				.Select(x => x.ToString().TrimStart('/').Trim())
				.Where(x => x.StartsWith("Automaty"))
				.ToList();

			foreach (string commentDirective in commentDirectives)
			{
				string[] strings = commentDirective.Split(new[] { ' ' }, 3);

				if (strings.Length != 3)
				{
					continue;
				}

				switch (strings[1].ToLower())
				{
					case "includefile":
						additionalFiles.Add(strings[2]);
						break;

					case "includefiles":
						foreach (string additionalFilePath in strings[2].Split(';'))
						{
							additionalFiles.Add(additionalFilePath);
						}

						break;

					case "includedirectory":
						additionalDirectories.Add(strings[2]);
						break;

					case "includedirectories":
						foreach (string additionalDirectoryPath in strings[2].Split(';'))
						{
							additionalDirectories.Add(additionalDirectoryPath);
						}

						break;
				}
			}

			return (additionalFiles, additionalDirectories);
		}

		protected IEnumerable<string> GetFilePathsFromDirectoryPath(string baseDirectoryPath, string directoryPath)
		{
			directoryPath = GetFullPath(baseDirectoryPath, directoryPath);

			foreach (string additionalFilePath in Directory.EnumerateFiles(directoryPath, "*.cs", SearchOption.AllDirectories))
			{
				yield return additionalFilePath;
			}
		}

		protected string GetFullPath(string baseDirectoryPath, string path)
		{
			return Path.GetFullPath(Path.Combine(baseDirectoryPath.ToPlatformSpecificPath(), path.ToPlatformSpecificPath()));
		}

		protected IEnumerable<MetadataReference> ResolveReferences(MetadataReferenceResolver metadataReferenceResolver)
		{
			List<MetadataReference> references = new List<MetadataReference>
			{
				MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location)
			};

			foreach (MetadataReference reference in MetadataReferences)
			{
				UnresolvedMetadataReference unresolved = reference as UnresolvedMetadataReference;

				if (unresolved != null)
				{
					ImmutableArray<PortableExecutableReference> resolved =
						metadataReferenceResolver.ResolveReference(unresolved.Reference, null, unresolved.Properties);

					references.AddRange(resolved);
				}
				else
				{
					references.Add(reference);
				}
			}

			return references;
		}
	}
}