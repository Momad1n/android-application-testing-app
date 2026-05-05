using Backend.Services;
using NUnit.Framework;

namespace Backend.UnitTests
{
    [TestFixture]
    public class PasswordPolicyTests
    {
        private string _testPassword; [SetUp]
        public void Setup()
        {
            _testPassword = string.Empty;
        }

        [Test]
        public void IsPasswordStrong_LengthIs8ButNoDigit_ReturnsFalse()
        {
            
            _testPassword = "password"; 
            
            bool result = AuthenticationService.IsPasswordStrong(_testPassword);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordStrong_NullOrEmptyPassword_ReturnsFalse()
        {
            bool resultNull = AuthenticationService.IsPasswordStrong(null);
            bool resultEmpty = AuthenticationService.IsPasswordStrong(string.Empty);

            Assert.That(resultNull, Is.False);
            Assert.That(resultEmpty, Is.False);
        }

        [Test]
        public void IsPasswordStrong_PasswordShorterThan8_ReturnsFalse()
        {
            // Arrange
            _testPassword = "Pass1"; 

            // Act
            bool result = AuthenticationService.IsPasswordStrong(_testPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordStrong_ValidPassword_ReturnsTrue()
        {
            // Arrange
            _testPassword = "StrongPassword123"; 

            // Act
            bool result = AuthenticationService.IsPasswordStrong(_testPassword);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}