using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Authentications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.API.Controllers
{

    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }
        // Login 
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto ,CancellationToken cancellationToken)
        {
            return ToActionResult (await authenticationService.LoginAsync(loginDto, cancellationToken));
        }

        [HttpPost("register")]

        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto ,CancellationToken ct )
        {
            return ToActionResult(await authenticationService.RegisterAsync(registerDto, ct));
        }

        // check Email is already exist or not
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmail([FromQuery] string email, CancellationToken ct)
        {
            return ToActionResult(await authenticationService.CheckEmailExistAsync(email, ct));
        }

        // Get Current User
        [Authorize]
        [HttpGet("currentUser")]
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
    }
}
