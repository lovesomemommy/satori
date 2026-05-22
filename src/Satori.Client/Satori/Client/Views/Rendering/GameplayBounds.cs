namespace Satori.Client.Views.Rendering;

public readonly struct GameplayBounds
{
	public int Width { get; init; }

	public int Height { get; init; }

	public static GameplayBounds FromViewport(FixedViewportRenderer viewport) =>
		new()
		{
			Width = viewport.VirtualWidth,
			Height = viewport.VirtualHeight
		};
}
