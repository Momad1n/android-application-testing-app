using Microsoft.AspNetCore.Authentication;
using NUnit.Framework;
using Backend.Services;

namespace Backend.UnitTests;

[TestFixture]
public class PasswordPolicyTests
{

    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void IsPasswordStrong_PasswordShorterThan8_ReturnsFalse()
    {
        bool result = Backend.Services.AuthenticationService.IsPasswordStrong("short1");

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsPasswordStrong_LengthIs8ButNoDigit_ReturnsFalse()
    {
        bool result = Backend.Services.AuthenticationService.IsPasswordStrong("password");

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsPasswordStrong_NullOrEmptyPassword_ReturnsFalse()
    {
        Assert.That(Backend.Services.AuthenticationService.IsPasswordStrong(null), Is.False);
        Assert.That(Backend.Services.AuthenticationService.IsPasswordStrong(""), Is.False);
    }

    [Test]
    public void IsPasswordStrong_ValidPassword_ReturnsTrue()
    {
        bool result = Backend.Services.AuthenticationService.IsPasswordStrong("StrongPass1");

        Assert.That(result, Is.True);
    }
}