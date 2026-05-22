using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Precepts;

namespace Satori.Core.Systems.Precepts;

public interface IPreceptHandler
{
	PreceptType Type { get; }

	bool AppliesTo(TrialSegmentDefinition segment);

	PreceptViolationResult Evaluate(PreceptContext context);
}
