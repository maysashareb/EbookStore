// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using EbookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace EbookStore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager; // Updated to use AppUser
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _environment;

        public RegisterConfirmationModel(UserManager<AppUser> userManager, IEmailSender emailSender, IWebHostEnvironment environment)
        {
            _userManager = userManager; // Updated to use AppUser
            _emailSender = emailSender;
            _environment = environment;
        }

        public string Email { get; set; }
        public bool DisplayConfirmAccountLink { get; set; }
        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Index");
            }

            returnUrl = returnUrl ?? Url.Content("~/");
            var user = await _userManager.FindByEmailAsync(email); // Updated to use AppUser
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;

            // Generate the confirmation token and URL
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            EmailConfirmationUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                protocol: Request.Scheme);

            if (_environment.IsDevelopment())
            {
                // Show the confirmation link directly in the development environment
                DisplayConfirmAccountLink = true;
            }
            else
            {
                // Send the confirmation email in production
                DisplayConfirmAccountLink = false;
                await _emailSender.SendEmailAsync(
                    email,
                    "Confirm Your Email",
                    $"Please confirm your account by <a href='{EmailConfirmationUrl}'>clicking here</a>.");
            }

            return Page();
        }
    }
}
