using E_Commerce.Infrastructure.Identity.Entities;
using E_Commerce.Infrastructure.Identity.Services;
using E_Commerce.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Cryptography;
using System.Text;
using Xunit;


namespace E_Commerce.Tests.Services
{
    public class RefreshTokenServiceTests
    {
        private static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            var storeMock = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(
                storeMock.Object, null, null, null, null, null, null, null, null);
            return mgr;
        }

        [Fact]
        public async Task CreateAsync_UserExists_ReturnsSuccessWithToken()
        {
           
            var fakeUser = new ApplicationUser
            {
                Email = "test@example.com",
                UserName = "test@example.com"
            };

            var userManagerMock = CreateMockUserManager();

            
            userManagerMock.Setup(m => m.FindByEmailAsync("test@example.com")).ReturnsAsync(fakeUser);

            userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            var sut = new RefreshTokenService(userManagerMock.Object);

            var result = await sut.CreateAsync("test@example.com");

            Assert.True(result.IsSuccess);
            Assert.False(string.IsNullOrWhiteSpace(result.data));
        }

        [Fact]
        public async Task CreateAsync_UserNotFound_ReturnsFailResult()
        {
            var userManagerMock = CreateMockUserManager();


            var sut = new RefreshTokenService(userManagerMock.Object);

            var result = await sut.CreateAsync("notfound@example.com");

            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
            Assert.Equal("UserNotFound", result.Errors[0].Code);
        }

        [Fact]
        public async Task RevokeAsync_UserExists_ReturnsSuccessAndClearsToken()
        {
            var fakeUser = new ApplicationUser
            {
                Email = "test@example.com",
                UserName = "test@example.com",
                RefreshTokenHash = "some-old-hash",
                RefreshTokenExpiresOn = DateTime.UtcNow.AddDays(3)
            };

            var userManagerMock = CreateMockUserManager();

            userManagerMock.Setup(m => m.FindByEmailAsync("test@example.com")).ReturnsAsync(fakeUser);

            userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            var sut = new RefreshTokenService(userManagerMock.Object);

            var result = await sut.RevokeAsync("test@example.com");

            Assert.True(result.IsSuccess);
            Assert.Null(fakeUser.RefreshTokenHash);
            Assert.Null(fakeUser.RefreshTokenExpiresOn);
        }

        [Fact]
        public async Task RevokeAsync_UserNotFound_ReturnsFailResult()
        {
            var userManagerMock = CreateMockUserManager();

            var sut = new RefreshTokenService(userManagerMock.Object);

            
            var result = await sut.RevokeAsync("notfound@example.com");

            Assert.False(result.IsSuccess);
            Assert.Equal("UserNotFound", result.Errors[0].Code);
        }

        [Fact]
        public async Task GetEmailAsync_ValidToken_ReturnsEmail()
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));

            var fakeUser = new ApplicationUser
            {
                Email = "test@example.com",
                RefreshTokenHash = hash,
                RefreshTokenExpiresOn = DateTime.UtcNow.AddDays(3) 
            };

            var users = new List<ApplicationUser> { fakeUser };
            var userManagerMock = CreateMockUserManager();

            userManagerMock.Setup(m => m.Users).Returns(new TestAsyncEnumerable<ApplicationUser>(users));

            var sut = new RefreshTokenService(userManagerMock.Object);

           
            var result = await sut.GetEmailAsync(refreshToken);

            Assert.True(result.IsSuccess);
            Assert.Equal("test@example.com", result.data);
        }

        [Fact]
        public async Task GetEmailAsync_ExpiredToken_ReturnsUnauthorized()
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));

            var fakeUser = new ApplicationUser
            {
                Email = "test@example.com",
                RefreshTokenHash = hash,
                RefreshTokenExpiresOn = DateTime.UtcNow.AddDays(-1) 
            };

            var users = new List<ApplicationUser> { fakeUser };
            var userManagerMock = CreateMockUserManager();

            userManagerMock.Setup(m => m.Users).Returns(new TestAsyncEnumerable<ApplicationUser>(users));

            var sut = new RefreshTokenService(userManagerMock.Object);

            var result = await sut.GetEmailAsync(refreshToken);

            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidRefreshToken", result.Errors[0].Code);
        }

        [Fact]
        public async Task GetEmailAsync_TokenNotFound_ReturnsUnauthorized()
        {
            var users = new List<ApplicationUser>(); 
            var userManagerMock = CreateMockUserManager();

            userManagerMock.Setup(m => m.Users).Returns(new TestAsyncEnumerable<ApplicationUser>(users));

            var sut = new RefreshTokenService(userManagerMock.Object);

            var result = await sut.GetEmailAsync("some-random-token-not-in-db");

            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidRefreshToken", result.Errors[0].Code);
        }
    }
}