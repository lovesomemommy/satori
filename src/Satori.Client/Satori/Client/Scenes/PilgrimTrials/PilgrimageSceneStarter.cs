using Microsoft.Extensions.DependencyInjection;
using Satori.Client.Content;
using Satori.Client.State;
using Satori.Core.Interfaces.Systems;

namespace Satori.Client.Scenes.PilgrimTrials;

public static class PilgrimageSceneStarter
{
	public static void StartFromHub(SceneContext context)
	{
		IPilgrimPilgrimageSystem requiredService = context.Services.GetRequiredService<IPilgrimPilgrimageSystem>();
		requiredService.BindPersistence(context.Session.Meta, context.Session.Save.Pilgrimage, context.Session.Save.Wisdom);
		requiredService.Start(PilgrimageContentFactory.Create());
		context.SceneManager.ChangeTo(GameStateType.PilgrimTrial);
	}
}
