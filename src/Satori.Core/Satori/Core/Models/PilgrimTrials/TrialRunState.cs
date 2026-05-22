using System;
using System.Collections.Generic;
using Satori.Core.Models.Wisdom;

namespace Satori.Core.Models.PilgrimTrials;

public sealed class TrialRunState
{
	public const int SegmentCount = 5;

	public TrialOutcome Outcome { get; set; } = TrialOutcome.InProgress;

	public TrialDefeatReason DefeatReason { get; set; } = TrialDefeatReason.None;

	public int CurrentSegmentIndex { get; set; }

	public TimeSpan RemainingTime { get; set; } = TimeSpan.FromMinutes(4.0) + TimeSpan.FromSeconds(44.0);

	public int RunKarma { get; set; }

	public HashSet<int> CollectedLotusIds { get; } = new HashSet<int>();

	public List<QuoteModel> UnlockedQuotes { get; } = new List<QuoteModel>();

	public string? LastDefeatMessageKey { get; set; }

	public bool IsActive => Outcome == TrialOutcome.InProgress;
}
