﻿namespace Automaty.Core.Logging
{
	public class NullLogger<T> : ILogger<T>
	{
		public void WriteDebug(string text)
		{
		}

		public void WriteError(string text)
		{
		}

		public void WriteInfo(string text)
		{
		}

		public void WriteWarning(string text)
		{
		}
	}
}