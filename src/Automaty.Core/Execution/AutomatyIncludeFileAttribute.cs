namespace Automaty.Core.Execution
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class AutomatyIncludeFileAttribute : Attribute
	{
		public string File { get; set; }
	}
}