using Microsoft.AspNetCore.Authentication;
using NUnit.Framework;
using Backend.Services;

namespace Backend.UnitTests;

[TestFixture]
public class PasswordPolicyTests
{
    private Backend.Services.AuthenticationService _service;

    [SetUp]
    public void Setup()
    {
        _service = new Backend.Services.AuthenticationService();
    }

    [Test]
    public void IsPasswordStrong_PasswordShorterThan8_ReturnsFalse()
    {
        bool result = _service.IsPasswordStrong("short1");

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsPasswordStrong_LengthIs8ButNoDigit_ReturnsFalse()
    {
        bool result = _service.IsPasswordStrong("password");

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsPasswordStrong_NullOrEmptyPassword_ReturnsFalse()
    {
        Assert.That(_service.IsPasswordStrong(null), Is.False);
        Assert.That(_service.IsPasswordStrong(""), Is.False);
    }

    [Test]
    public void IsPasswordStrong_ValidPassword_ReturnsTrue()
    {
        bool result = _service.IsPasswordStrong("StrongPass1");

        Assert.That(result, Is.True);
    }
}