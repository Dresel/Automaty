namespace Automaty.Core.Execution
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class AutomatyIncludeDirectoryAttribute : Attribute
	{
		public string Directory { get; set; }
	}
}