﻿namespace Automaty.Samples.HelloWorld.MSBuild
{
	using Automaty.Common.Output;

	public class HelloWorld
	{
		public void Execute(IScriptContext context)
		{
			context.Output.Current.WriteLine("// Hello World!");
		}
	}
}