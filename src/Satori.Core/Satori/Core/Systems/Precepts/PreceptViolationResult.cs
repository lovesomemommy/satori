using Satori.Core.Models.Precepts;

namespace Satori.Core.Systems.Precepts;

public sealed class PreceptViolationResult
{
	public static PreceptViolationResult None { get; } = new PreceptViolationResult(isViolated: false, PreceptType.NoKilling, string.Empty);

	public bool IsViolated { get; }

	public PreceptType PreceptType { get; }

	public string MessageKey { get; }

	public PreceptViolationResult(bool isViolated, PreceptType preceptType, string messageKey)
	{
		IsViolated = isViolated;
		PreceptType = preceptType;
		MessageKey = messageKey;
	}
}
