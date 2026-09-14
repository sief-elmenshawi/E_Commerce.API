using E_Commerce.Application.DTOs.Authentications;
using E_Commerce.Application.Validators.Authentications;
using Xunit;

namespace E_Commerce.Tests.Validators
{
    public class RegisterDtoValidatorTests
    {
        private readonly RegisterDtoValidator _validator = new();

        [Theory]
        [InlineData("01012345678")]
        [InlineData("01112345678")]
        [InlineData("01212345678")]
        [InlineData("01512345678")]
        public void ValidEgyptianPhone_Passes(string phone)
        {
            var dto = ValidDto();
            dto.PhoneNumber = phone;

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("01412345678")]
        [InlineData("0101234")]
        [InlineData("0101234567a")]
        [InlineData("11012345678")]
        [InlineData("010123456789")]
        public void InvalidPhones_Fail(string phone)
        {
            var dto = ValidDto();
            dto.PhoneNumber = phone;

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void EmptyPhone_IsOptional()
        {
            var dto = ValidDto();
            dto.PhoneNumber = null;

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("1234567")]
        [InlineData("password")]
        [InlineData("PASSWORD1")]
        [InlineData("Passw1")]
        [InlineData("nouppercase1")]
        public void WeakPassword_Fails(string password)
        {
            var dto = ValidDto();
            dto.Password = password;

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void EmptyUserName_Fails()
        {
            var dto = ValidDto();
            dto.UserName = "";

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void ValidDto_Passes()
        {
            var result = _validator.Validate(ValidDto());

            Assert.True(result.IsValid);
        }

        private static RegisterDto ValidDto() => new()
        {
            Email = "user@example.com",
            Password = "Str0ng!Pass",
            UserName = "username",
            DisplayName = "User Name",
            PhoneNumber = "01012345678"
        };
    }
}
