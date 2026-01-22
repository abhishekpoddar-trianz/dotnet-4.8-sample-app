using Xunit;
using FluentAssertions;

namespace DescopeSampleApp.UnitTests;

public class SampleTest
{
    [Fact]
    public void Sample_Test_Should_Pass()
    {
        // Arrange
        var expected = true;

        // Act
        var actual = true;

        // Assert
        actual.Should().Be(expected);
    }
}
