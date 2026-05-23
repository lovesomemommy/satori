namespace Satori.Client.Views.Rendering;

public static class ClientAssetPaths
{
	public static IEnumerable<string> InFolder(string folderName, string fileName)
	{
		yield return Path.Combine(AppContext.BaseDirectory, folderName, fileName);
	}

	public static IEnumerable<string> PilgrimImage(string fileName) =>
		InFolder("PilgrimImages", fileName);

	public static IEnumerable<string> QuoteImage(string fileName) =>
		InFolder("QuoteImages", fileName);

	public static IEnumerable<string> QuoteImageById(string quoteId)
	{
		string fileName = quoteId + ".png";
		yield return Path.Combine(AppContext.BaseDirectory, "QuoteImages", fileName);
		yield return Path.Combine(AppContext.BaseDirectory, "assets", "quotes", fileName);
	}
}
