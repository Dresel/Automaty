namespace Automaty.Samples.EFCore2.MSBuild
{
	using SmartFormat.Core.Extensions;

	public class FirstCharacterToLowerFormatter : IFormatter
	{
		public string[] Names { get; set; } = { "firstcharactertolower", "fctl" };

		public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
		{
			bool canHandleThisInput = formattingInfo.CurrentValue is string;

			if (!canHandleThisInput)
			{
				return false;
			}

			formattingInfo.Write(((string)formattingInfo.CurrentValue).FirstCharacterToLower());

			return true;
		}
	}
}