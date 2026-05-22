using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.Services.Hub;
using Satori.Client.Services.PilgrimTrials;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.Views.Rendering;
using Satori.Core.Interfaces.Services;

namespace Satori.Client.Scenes;

public sealed class SceneContext
{
	public required Game Game { get; init; }

	public required GraphicsDevice GraphicsDevice { get; init; }

	public required SpriteBatch SpriteBatch { get; init; }

	public required Texture2D Pixel { get; init; }

	public required FixedViewportRenderer Viewport { get; init; }

	public required GameplayBounds GameplayBounds { get; init; }

	public required ILocalizationService Localization { get; init; }

	public required SceneManager SceneManager { get; init; }

	public required GameStateMachine StateMachine { get; init; }

	public required GameSession Session { get; init; }

	public required IServiceProvider Services { get; init; }

	public required TextRenderingService Text { get; init; }

	public required ObstacleSpriteCatalog ObstacleSprites { get; init; }

	public required PilgrimSpriteCatalog PilgrimSprites { get; init; }

	public required GardenSpriteCatalog GardenSprites { get; init; }
}
