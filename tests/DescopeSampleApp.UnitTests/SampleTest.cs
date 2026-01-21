using Xunit;
using FluentAssertions;

namespace DescopeSampleApp.UnitTests;

public class SampleTest
{
    [Fact]
    public void SampleTest_ShouldPass()
    {
        var result = true;
        result.Should().BeTrue();
    }
}
