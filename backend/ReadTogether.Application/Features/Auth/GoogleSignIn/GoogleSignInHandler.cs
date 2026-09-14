using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ReadTogether.API.Configuration;
using ReadTogether.Domain.Configuration;
using ReadTogether.Domain.Entities;
using ReadTogether.Infrastructure.Interfaces;

namespace ReadTogether.Application.Features.Auth.GoogleSignIn
{
    public class GoogleSignInHandler : IRequestHandler<GoogleSignInCommand, GoogleSignInResult>
    {
        private readonly IGoogleTokenValidator tokenValidator;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJwtTokenService tokenService;
        private readonly IBookshelfRepository bookshelfRepository;
        private readonly string[] defaultBookshelves;

        public GoogleSignInHandler(IGoogleTokenValidator tokenValidator, UserManager<ApplicationUser> userManager, IJwtTokenService tokenService, IBookshelfRepository bookshelfRepository, IOptions<BookshelvesConfiguration> bookshelvesConfig)
        {
            this.tokenValidator = tokenValidator;
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.bookshelfRepository = bookshelfRepository;
            this.defaultBookshelves = bookshelvesConfig.Value.DefaultBookshelves;
        }

        public async Task<GoogleSignInResult> Handle(GoogleSignInCommand request, CancellationToken cancellationToken)
        {
            // Validate Token Using Google's Validator
            var googleUser = await tokenValidator.ValidateToken(request.idToken, cancellationToken);

            // Ensure email is verified
            if (!googleUser.EmailVerified)
            {
                // TODO: Throw custom exception
                throw new Exception("Email must be verified in Google before signing up");
            }

            // Check if user already exists in database
            var user = await userManager.FindByEmailAsync(googleUser.Email);

            // If user does not exist, create it
            if (user is null)
            {
                user = new ApplicationUser
                {
                    Email = googleUser.Email,
                    UserName = googleUser.Email,
                    EmailConfirmed = googleUser.EmailVerified,
                    ImageUrl = googleUser.PictureUrl,
                    DateCreated = DateTime.UtcNow
                };

                var res = await userManager.CreateAsync(user);
                if (!res.Succeeded)
                {
                    throw new Exception("Failed to create account: " + string.Join(", ", res.Errors.Select(e => e.Description)));
                }

                if (defaultBookshelves.Length > 0)
                {
                    // Create default bookshelves for the new user
                    foreach (var bookshelf in defaultBookshelves)
                    {
                        await bookshelfRepository.CreateBookshelf(bookshelf, user.Id, cancellationToken, true);
                    }
                }
                else
                    Console.WriteLine("No default bookshelves configured. Skipping bookshelf creation.");

            }
            else
            {
                // Check if user is already linked to this google account
                var logins = await userManager.GetLoginsAsync(user);
                var isLinked = logins.Any(login => login.LoginProvider == "Google" && login.ProviderKey == googleUser.Subject);

                // If not linked then add the link
                if (!isLinked)
                {
                    var loginInfo = new UserLoginInfo("Google", googleUser.Subject, "Google");

                    var linkResult = await userManager.AddLoginAsync(user, loginInfo);

                    if (!linkResult.Succeeded)
                    {
                        // TODO: Throw custom exception
                        throw new Exception("Linking to account failed");
                    }
                }
            }

            // Generate new JWT token
            var tokenResponse = tokenService.GenerateToken(user);


            // Return results along with token
            return new GoogleSignInResult
            {
                Email = googleUser.Email,
                UserId = user.Id,
                UserName = user.UserName ?? "",
                JwtToken = tokenResponse.Token,
                ExpiresAt = tokenResponse.ExpiresAt
            };
        }
    }
}