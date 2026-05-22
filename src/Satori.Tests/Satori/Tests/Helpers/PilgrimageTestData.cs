using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Precepts;

namespace Satori.Tests.Helpers;

public static class PilgrimageTestData
{
	public static PilgrimPilgrimageDefinition CreateDefault()
	{
		List<TrialSegmentDefinition> list = new List<TrialSegmentDefinition>();
		for (int i = 0; i < 5; i++)
		{
			SegmentFocus focus = (SegmentFocus)i;
			TrialSegmentDefinition obj = new TrialSegmentDefinition
			{
				SegmentIndex = i,
				TitleKey = $"pilgrim.segment.0{i + 1}.title",
				Focus = focus,
				Width = 20,
				Height = 15,
				Spawn = new SpawnPoint
				{
					X = 1,
					Y = 7
				},
				ExitPortal = new PortalPoint
				{
					X = 18,
					Y = 7
				},
				Traps = ((i == 2) ? new List<TrapModel>
				{
					new TrapModel
					{
						Tile = new TilePoint(10, 2)
					}
				} : new List<TrapModel>())
			};
			List<DecoyTrailModel> list2;
			if (i != 2)
			{
				list2 = new List<DecoyTrailModel>();
			}
			else
			{
				list2 = new List<DecoyTrailModel>();
				List<DecoyTrailModel> obj2 = list2;
				DecoyTrailModel decoyTrailModel = new DecoyTrailModel();
				int num = 5;
				List<TilePoint> list3 = new List<TilePoint>(num);
				CollectionsMarshal.SetCount(list3, num);
				Span<TilePoint> span = CollectionsMarshal.AsSpan(list3);
				int num2 = 0;
				span[num2] = new TilePoint(9, 5);
				num2++;
				span[num2] = new TilePoint(9, 4);
				num2++;
				span[num2] = new TilePoint(9, 3);
				num2++;
				span[num2] = new TilePoint(9, 2);
				num2++;
				span[num2] = new TilePoint(10, 2);
				num2++;
				decoyTrailModel.Path = list3;
				obj2.Add(decoyTrailModel);
			}
			obj.DecoyTrails = list2;
			obj.Obstacles = ((i == 0) ? new List<ObstacleModel>
			{
				new ObstacleModel
				{
					Type = ObstacleType.Harm,
					Tile = new TilePoint(6, 5)
				}
			} : new List<ObstacleModel>());
			obj.Lotuses = new List<LotusModel>
			{
				new LotusModel
				{
					Id = i + 1,
					Type = LotusType.Common,
					SegmentIndex = i,
					TileX = 10,
					TileY = 5
				}
			};
			list.Add(obj);
		}
		return new PilgrimPilgrimageDefinition
		{
			TitleKey = "pilgrim.title",
			Segments = list
		};
	}
}
