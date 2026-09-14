using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using ReadTogether.API.InputModels.Auth;
using ReadTogether.Application.Features.Auth.GoogleSignIn;
using ReadTogether.Domain.Entities;

namespace ReadTogether.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;

        public AuthController(UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _userManager = userManager;
            _mediator = mediator;
        }

        /* [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInputModel inputModel)
        {
            var user = new ApplicationUser
            {
                Email = inputModel.Email,
                UserName = inputModel.UserName,
            };

            var res = await _userManager.CreateAsync(user, inputModel.Password);

            if (res.Succeeded)
            {
                return Ok("User created successfully");
            }
            return BadRequest(res.Errors);
        } */

        /* [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn()
        {
            throw new NotImplementedException();
        } */

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            string? email = User.FindFirstValue(ClaimTypes.Email);
            string? name = User.FindFirstValue(ClaimTypes.Name)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Name)
                ?? User.FindFirstValue("name");
            return Ok($"You are authenticated as {name} ({email})");
        }


        [AllowAnonymous]
        [HttpPost("google")]
        public async Task<IActionResult> GoogleSignInOrRegister([FromBody] GoogleSignInInputModel inputModel, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received Google Sign-In request with ID Token: {inputModel.IdToken}");
            var command = new GoogleSignInCommand(inputModel.IdToken);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
    }


}