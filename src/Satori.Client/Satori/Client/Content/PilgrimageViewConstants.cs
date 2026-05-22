namespace Satori.Client.Content;

public static class PilgrimageViewConstants
{
	public const int TileSize = 15;

	public const int GridWidth = 18;

	public const int GridHeight = 11;

	public static int MapPixelWidth => 270;

	public static int MapPixelHeight => 165;

	public static int MapOffsetX => (320 - MapPixelWidth) / 2;

	public static int MapOffsetY => (180 - MapPixelHeight) / 2;
}
