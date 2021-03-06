﻿namespace Automaty.Core.Execution
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Threading;
	using Automaty.Common.Execution;
	using Automaty.Common.Logging;
	using Automaty.Common.Output;
	using Automaty.Core.Logging;
	using Microsoft.Build.Utilities;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.Emit;

#if NETSTANDARD1_6
	using System.Runtime.Loader;
#endif

	public class ScriptRunner
	{
		public ScriptRunner(ScriptCompiler scriptCompiler) : this(scriptCompiler, new NullLoggerFactory())
		{
		}

		public ScriptRunner(ScriptCompiler scriptCompiler, ILoggerFactory loggerFactory)
		{
			ScriptCompiler = scriptCompiler;
			LoggerFactory = loggerFactory;
			Logger = loggerFactory.CreateLogger<ScriptRunner>();
		}

		public ILogger<ScriptRunner> Logger { get; set; }

		public ILoggerFactory LoggerFactory { get; set; }

		public ScriptCompiler ScriptCompiler { get; set; }

		public void Run(string sourceFilePath, string projectFilePath = "")
		{
			Compilation compilation = ScriptCompiler.Compile(sourceFilePath);

			using (MemoryStream msLibrary = new MemoryStream())
			using (MemoryStream msSymbols = new MemoryStream())
			{
				Logger.WriteDebug($"Emitting compilation for '{sourceFilePath}'.");

				EmitResult result = compilation.Emit(msLibrary, msSymbols,
					options: new EmitOptions().WithDebugInformationFormat(DebugInformationFormat.PortablePdb));

				if (!result.Success)
				{
					Logger.WriteError("Some errors happened while emmitting the compilation.");

					foreach (Diagnostic diagnostic in result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError ||
						diagnostic.Severity == DiagnosticSeverity.Error))
					{
						Logger.WriteWarning($"{diagnostic.Id}: {diagnostic.GetMessage()}");
					}
				}
				else
				{
					msLibrary.Seek(0, SeekOrigin.Begin);
					msSymbols.Seek(0, SeekOrigin.Begin);

					Logger.WriteDebug("Loading assembly.");

#if NETSTANDARD1_6
					Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(msLibrary, msSymbols);
#else
					Assembly assembly = Assembly.Load(msLibrary.ToArray(), msSymbols.ToArray());
#endif

					Logger.WriteDebug("Searching for Automaty hosts.");

					IEnumerable<Type> types = assembly.GetTypes().Where(x => x.GetTypeInfo().IsClass);

					foreach (Type type in types)
					{
						IEnumerable<MethodInfo> methodInfos = type.GetMethods()
							.Where(x => x.Name == nameof(IAutomatyHost.Execute) && (x.GetParameters().Length == 0 ||
								x.GetParameters().Length == 1 && x.GetParameters().Single().ParameterType == typeof(IScriptContext)));

						foreach (MethodInfo methodInfo in methodInfos)
						{
							if (methodInfo.GetParameters().Length == 0)
							{
								Logger.WriteInfo($"Invoking {type}->{methodInfo.Name}.");

								methodInfo.Invoke(
									methodInfo.IsStatic ? type : Activator.CreateInstance(type),
									new object[] { });
							}
							else
							{
								using (ScriptContext scriptContext = new ScriptContext(sourceFilePath, projectFilePath, LoggerFactory))
								{
									Logger.WriteInfo($"Invoking {type}->{methodInfo.Name}.");

									methodInfo.Invoke(
										methodInfo.IsStatic ? type : Activator.CreateInstance(type),
										new object[] { scriptContext });
								}
							}
						}
					}
				}
			}
		}
	}
}