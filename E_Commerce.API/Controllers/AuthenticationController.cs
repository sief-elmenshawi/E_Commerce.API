using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Authentications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace E_Commerce.API.Controllers
{

    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IRefreshTokenService refreshTokenService;

        public AuthenticationController(IAuthenticationService authenticationService , IRefreshTokenService refreshTokenService)
        {
            this.authenticationService = authenticationService;
            this.refreshTokenService = refreshTokenService;
        }
        // Login 
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto ,CancellationToken cancellationToken)
        {
            var loginResult = await authenticationService.LoginAsync( loginDto,cancellationToken);

            if (!loginResult.IsSuccess)
            {
                return ToActionResult(loginResult);
            }

            var refreshTokenResult = await refreshTokenService.CreateAsync(loginResult.data.Email,cancellationToken);

            if (!refreshTokenResult.IsSuccess)
            {
                return ToProblem(refreshTokenResult.Errors);
            }

            SetRefreshTokenCookie(refreshTokenResult.data);

            return Ok(loginResult.data);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto ,CancellationToken ct )
        {
            var registerResult = await authenticationService.RegisterAsync(registerDto,ct);

            if (!registerResult.IsSuccess)
            {
                return ToActionResult(registerResult);
            }

            var refreshTokenResult = await refreshTokenService.CreateAsync( registerResult.data.Email,ct);

            if (!refreshTokenResult.IsSuccess)
            {
                return ToProblem(refreshTokenResult.Errors);
            }

            SetRefreshTokenCookie(refreshTokenResult.data);

            return Ok(registerResult.data);
        }

        [HttpPost("refreshtoken")]
        public async Task<ActionResult<UserDto>> RefreshToken( CancellationToken ct)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized();
            }

            var emailResult = await refreshTokenService.GetEmailAsync(refreshToken,ct);
            if (!emailResult.IsSuccess)
            {
                return ToProblem(emailResult.Errors);
            }
            var userResult = await authenticationService.GetCurrentUserAsync(emailResult.data,ct);
            if (!userResult.IsSuccess)
            {
                return ToActionResult(userResult);
            }
            var newRefreshTokenResult = await refreshTokenService.CreateAsync( emailResult.data,ct);
            if (!newRefreshTokenResult.IsSuccess)
            {
                return ToProblem(newRefreshTokenResult.Errors);
            }

            SetRefreshTokenCookie(newRefreshTokenResult.data);

            return Ok(userResult.data);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var emailResult = await refreshTokenService.GetEmailAsync(refreshToken,ct);

                if (emailResult.IsSuccess)
                {
                    var revokeResult = await refreshTokenService.RevokeAsync(emailResult.data,ct);

                    if (!revokeResult.IsSuccess)
                    {
                        return ToProblem(revokeResult.Errors);
                    }
                }
            }

            Response.Cookies.Delete("refreshToken",new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Path = "/api/Authentication"
            });

            return NoContent();
        }

        // check Email is already exist or not
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmail([FromQuery] string email, CancellationToken ct)
        {
            return ToActionResult(await authenticationService.CheckEmailExistAsync(email, ct));
        }

        // Get Current User
        [Authorize]
        [HttpGet("currentuser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken ct)
        {
            return ToActionResult(await authenticationService.GetCurrentUserAsync(GetEmailFromToken(), ct));
        }
        // Get Current User Address
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress(CancellationToken ct)
        {
            return ToActionResult(await authenticationService.GetUserAddressAsync(GetEmailFromToken(), ct));
        }
        // Update Current User Address
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateCurrentUserAddress([FromBody] AddressDto addressDto, CancellationToken ct)
        {
            return ToActionResult(await authenticationService.UpsertUserAddressAsync(GetEmailFromToken(), addressDto, ct));
        }



        private void SetRefreshTokenCookie(string refreshToken)
        {
            Response.Cookies.Append("refreshToken",refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    IsEssential = true,
                    Path = "/api/Authentication"
            });
        }
    }
}
