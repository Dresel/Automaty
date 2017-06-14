namespace Automaty.Common.Execution
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class AutomatyIncludeDirectoriesAttribute : Attribute
	{
		public string[] Directories { get; set; }
	}
}