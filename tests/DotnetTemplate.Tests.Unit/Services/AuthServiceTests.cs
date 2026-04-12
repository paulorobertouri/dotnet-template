using DotnetTemplate.Application.Services;
using FluentAssertions;
using NUnit.Framework;

namespace DotnetTemplate.Tests.Unit.Services;

[TestFixture]
public sealed class AuthServiceTests
{
    private const string Secret = "test-super-secret-key-at-least-32-chars-ok";
    private const string Issuer = "unit-test-issuer";
    private const string Audience = "unit-test-audience";

    private AuthService _sut = null!;

    [SetUp]
    public void SetUp() =>
        _sut = new AuthService(Secret, Issuer, Audience, expirationSeconds: 3600);

    [Test]
    public void IssueToken_ShouldReturnNonEmptyString()
    {
        var token = _sut.IssueToken("user@example.com");

        token.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void ValidateToken_WithValidToken_ShouldReturnSubject()
    {
        var token = _sut.IssueToken("user@example.com");

        var subject = _sut.ValidateToken(token);

        subject.Should().Be("user@example.com");
    }

    [Test]
    public void ValidateToken_WithInvalidToken_ShouldReturnNull()
    {
        var subject = _sut.ValidateToken("not.a.valid.token");

        subject.Should().BeNull();
    }

    [Test]
    public void ValidateToken_WithTokenFromDifferentSecret_ShouldReturnNull()
    {
        var otherService = new AuthService(
            "completely-different-secret-key-32-chars-min",
            Issuer,
            Audience);

        var token = otherService.IssueToken("user@example.com");

        var subject = _sut.ValidateToken(token);

        subject.Should().BeNull();
    }

    [Test]
    public void ValidateToken_WithExpiredToken_ShouldReturnNull()
    {
        var expiredService = new AuthService(Secret, Issuer, Audience, expirationSeconds: -1);

        var token = expiredService.IssueToken("user@example.com");

        var subject = _sut.ValidateToken(token);

        subject.Should().BeNull();
    }

    [Test]
    public void IssueToken_DifferentSubjects_ReturnDifferentTokens()
    {
        var token1 = _sut.IssueToken("alice@example.com");
        var token2 = _sut.IssueToken("bob@example.com");

        token1.Should().NotBe(token2);
    }
}
