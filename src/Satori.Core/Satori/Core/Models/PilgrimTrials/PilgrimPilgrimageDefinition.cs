using System;
using System.Collections.Generic;

namespace Satori.Core.Models.PilgrimTrials;

public sealed class PilgrimPilgrimageDefinition
{
	public string TitleKey { get; set; } = "pilgrim.title";

	public IReadOnlyList<TrialSegmentDefinition> Segments { get; set; } = Array.Empty<TrialSegmentDefinition>();
}
