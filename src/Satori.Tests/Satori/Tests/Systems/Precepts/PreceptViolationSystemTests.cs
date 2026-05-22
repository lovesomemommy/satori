using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Precepts;
using Satori.Core.Systems.PilgrimTrials;
using Satori.Core.Systems.Precepts;
using Satori.Core.Systems.Precepts.Handlers;
using Satori.Tests.Helpers;
using Xunit;

namespace Satori.Tests.Systems.Precepts;

public sealed class PreceptViolationSystemTests
{
	[Fact]
	public void Evaluate_HarmObstacleOnNoKillingSegment_ReturnsViolation()
	{
		PreceptViolationSystem preceptViolationSystem = CreateSystem();
		TrialSegmentDefinition segment = CreateSegment(SegmentFocus.NoKilling, ObstacleType.Harm, 6, 5);
		PreceptViolationResult preceptViolationResult = preceptViolationSystem.Evaluate(segment, 6, 5);
		Assert.True(preceptViolationResult.IsViolated);
		Assert.Equal(PreceptType.NoKilling, preceptViolationResult.PreceptType);
		Assert.Equal("precept.no_killing.violation", preceptViolationResult.MessageKey);
	}

	[Fact]
	public void Evaluate_HarmObstacleOnOtherSegment_ReturnsNone()
	{
		PreceptViolationSystem preceptViolationSystem = CreateSystem();
		TrialSegmentDefinition segment = CreateSegment(SegmentFocus.NoStealing, ObstacleType.Harm, 6, 5);
		PreceptViolationResult preceptViolationResult = preceptViolationSystem.Evaluate(segment, 6, 5);
		Assert.False(preceptViolationResult.IsViolated);
	}

	[Fact]
	public void GetHintKey_ReturnsLocalizedKeyForFocus()
	{
		PreceptViolationSystem preceptViolationSystem = CreateSystem();
		TrialSegmentDefinition segment = CreateSegment(SegmentFocus.Celibacy, ObstacleType.Temptation, 1, 1);
		Assert.Equal("precept.celibacy.hint", preceptViolationSystem.GetHintKey(segment));
	}

	private static PreceptViolationSystem CreateSystem()
	{
		ObstacleSystem obstacles = new ObstacleSystem();
		IPreceptHandler[] handlers = new IPreceptHandler[4]
		{
			new ObstaclePreceptHandler(obstacles, PreceptType.NoKilling, SegmentFocus.NoKilling, ObstacleType.Harm, "precept.no_killing.violation"),
			new ObstaclePreceptHandler(obstacles, PreceptType.NoStealing, SegmentFocus.NoStealing, ObstacleType.Temptation, "precept.no_stealing.violation"),
			new ObstaclePreceptHandler(obstacles, PreceptType.NoIntoxication, SegmentFocus.NoIntoxication, ObstacleType.Mist, "precept.no_intoxication.violation"),
			new ObstaclePreceptHandler(obstacles, PreceptType.Celibacy, SegmentFocus.Celibacy, ObstacleType.Temptation, "precept.celibacy.violation")
		};
		return new PreceptViolationSystem(handlers);
	}

	private static TrialSegmentDefinition CreateSegment(SegmentFocus focus, ObstacleType type, int x, int y)
	{
		TrialSegmentDefinition trialSegmentDefinition = PilgrimageTestData.CreateDefault().Segments[0];
		trialSegmentDefinition.Focus = focus;
		int num = 1;
		List<ObstacleModel> list = new List<ObstacleModel>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<ObstacleModel> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = new ObstacleModel
		{
			Type = type,
			Tile = new TilePoint(x, y)
		};
		num2++;
		trialSegmentDefinition.Obstacles = list;
		return trialSegmentDefinition;
	}
}
