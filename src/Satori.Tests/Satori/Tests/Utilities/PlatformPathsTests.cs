using System;
using Satori.Core.Utilities;
using Xunit;

namespace Satori.Tests.Utilities;

public sealed class PlatformPathsTests
{
	[Fact]
	public void GetSaveDirectory_ReturnsNonEmptyPathContainingSatori()
	{
		string saveDirectory = PlatformPaths.GetSaveDirectory();
		Assert.False(string.IsNullOrWhiteSpace(saveDirectory));
		Assert.Contains("satori", saveDirectory, StringComparison.OrdinalIgnoreCase);
	}
}
