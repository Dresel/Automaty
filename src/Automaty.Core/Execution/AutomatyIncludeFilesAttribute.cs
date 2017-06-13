namespace Automaty.Core.Execution
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class AutomatyIncludeFilesAttribute : Attribute
	{
		public string[] Files { get; set; }
	}
}