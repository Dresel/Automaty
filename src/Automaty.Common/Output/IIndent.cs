namespace Automaty.Common.Output
{
	using System;

	public interface IIndent : IDisposable
	{
		int IndentLevel { get; set; }
	}
}