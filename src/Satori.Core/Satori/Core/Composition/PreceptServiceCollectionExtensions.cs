using System;
using Microsoft.Extensions.DependencyInjection;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Precepts;
using Satori.Core.Systems.PilgrimTrials;
using Satori.Core.Systems.Precepts;
using Satori.Core.Systems.Precepts.Handlers;

namespace Satori.Core.Composition;

internal static class PreceptServiceCollectionExtensions
{
	public static IServiceCollection AddPreceptHandlers(this IServiceCollection services)
	{
		services.AddSingleton<ObstacleSystem>();
		services.AddSingleton((Func<IServiceProvider, IPreceptHandler>)((IServiceProvider sp) => new ObstaclePreceptHandler(sp.GetRequiredService<ObstacleSystem>(), PreceptType.NoKilling, SegmentFocus.NoKilling, ObstacleType.Harm, "precept.no_killing.violation")));
		services.AddSingleton((Func<IServiceProvider, IPreceptHandler>)((IServiceProvider sp) => new ObstaclePreceptHandler(sp.GetRequiredService<ObstacleSystem>(), PreceptType.NoStealing, SegmentFocus.NoStealing, ObstacleType.Temptation, "precept.no_stealing.violation")));
		services.AddSingleton((Func<IServiceProvider, IPreceptHandler>)((IServiceProvider sp) => new ObstaclePreceptHandler(sp.GetRequiredService<ObstacleSystem>(), PreceptType.NoIntoxication, SegmentFocus.NoIntoxication, ObstacleType.Mist, "precept.no_intoxication.violation")));
		services.AddSingleton((Func<IServiceProvider, IPreceptHandler>)((IServiceProvider sp) => new ObstaclePreceptHandler(sp.GetRequiredService<ObstacleSystem>(), PreceptType.Celibacy, SegmentFocus.Celibacy, ObstacleType.Temptation, "precept.celibacy.violation")));
		services.AddSingleton((IServiceProvider sp) => new PreceptViolationSystem(sp.GetServices<IPreceptHandler>()));
		return services;
	}
}
