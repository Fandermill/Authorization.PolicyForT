using FluentAssertions;

namespace Authorization.PolicyForT.Tests.UnitTests;

public class AuthorizationResultTests
{
    [Fact]
    public void All_authorized_results_merge_into_authorized_result()
    {
        var resultA = new AuthorizationResult(true);
        var resultB = new AuthorizationResult(true);

        var mergedResult = AuthorizationResult.Merge(new[] { resultA, resultB });

        mergedResult.IsAuthorized.Should().BeTrue();
    }

    [Fact]
    public void All_failed_results_merge_into_failed_result()
    {
        var resultA = new AuthorizationResult(false);
        var resultB = new AuthorizationResult(false);

        var mergedResult = AuthorizationResult.Merge(new[] { resultA, resultB });

        mergedResult.IsAuthorized.Should().BeFalse();
    }

    [Fact]
    public void At_least_one_failed_result_merges_to_failed_result()
    {
        var resultA = new AuthorizationResult(true);
        var resultB = new AuthorizationResult(false);

        var mergedResult = AuthorizationResult.Merge(new[] { resultA, resultB });

        mergedResult.IsAuthorized.Should().BeTrue();
    }


}
