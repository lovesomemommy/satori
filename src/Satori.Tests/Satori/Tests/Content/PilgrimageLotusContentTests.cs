using Satori.Client.Content;
using Satori.Core.Models.Lotus;

namespace Satori.Tests.Content;

public sealed class PilgrimageLotusContentTests
{
	[Fact]
	public void PilgrimageDefinition_HasFifteenLotusesAcrossFiveSegments()
	{
		var definition = PilgrimageContentFactory.Create();

		Assert.Equal(15, definition.Segments.Sum(segment => segment.Lotuses.Count));
		Assert.All(definition.Segments, segment => Assert.Equal(3, segment.Lotuses.Count));
	}

	[Fact]
	public void PilgrimageDefinition_HasOneQuotedLotusPerSegment()
	{
		var definition = PilgrimageContentFactory.Create();

		Assert.Equal(5, definition.Segments.Sum(segment => segment.Lotuses.Count(lotus => lotus.HasQuote)));
		Assert.All(definition.Segments, segment =>
		{
			Assert.Single(segment.Lotuses, lotus => lotus.HasQuote);
			Assert.Equal(2, segment.Lotuses.Count(lotus => !lotus.HasQuote));
		});
	}

	[Fact]
	public void PilgrimageDefinition_AssignsSequentialLotusIds()
	{
		var definition = PilgrimageContentFactory.Create();
		var lotusIds = definition.Segments
			.SelectMany(segment => segment.Lotuses)
			.Select(lotus => lotus.Id)
			.OrderBy(id => id)
			.ToArray();

		Assert.Equal(Enumerable.Range(1, 15).ToArray(), lotusIds);
	}
}
