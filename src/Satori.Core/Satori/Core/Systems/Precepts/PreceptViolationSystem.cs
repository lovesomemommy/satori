using System.Collections.Generic;
using System.Linq;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Precepts;

namespace Satori.Core.Systems.Precepts;

public sealed class PreceptViolationSystem
{
	private readonly IReadOnlyList<IPreceptHandler> _handlers;

	public PreceptViolationSystem(IEnumerable<IPreceptHandler> handlers)
	{
		_handlers = handlers.ToList();
	}

	public PreceptViolationResult Evaluate(TrialSegmentDefinition segment, int tileX, int tileY)
	{
		PreceptContext context = new PreceptContext(segment, tileX, tileY);
		foreach (IPreceptHandler handler in _handlers)
		{
			if (handler.AppliesTo(segment))
			{
				PreceptViolationResult preceptViolationResult = handler.Evaluate(context);
				if (preceptViolationResult.IsViolated)
				{
					return preceptViolationResult;
				}
			}
		}
		return PreceptViolationResult.None;
	}

	public string GetHintKey(TrialSegmentDefinition segment) =>
		segment.Focus switch
		{
			SegmentFocus.NoKilling => "precept.no_killing.hint",
			SegmentFocus.NoStealing => "precept.no_stealing.hint",
			SegmentFocus.DecoyTrails => "precept.decoy_trails.hint",
			SegmentFocus.NoIntoxication => "precept.no_intoxication.hint",
			SegmentFocus.Celibacy => "precept.celibacy.hint",
			_ => string.Empty
		};
}
