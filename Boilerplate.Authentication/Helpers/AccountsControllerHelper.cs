using Boilerplate.Authentication.Models.Constants;
using Microsoft.AspNetCore.Http;
using System;

namespace Boilerplate.Authentication.Helpers
{
    public static class AccountsControllerHelper
    {
        public static string GetIpAddress(HttpRequest request)
        {
            if (request.Headers.TryGetValue(AccountConstants.HEADER_KEY, out var headerValeu))
            {
                return headerValeu;
            }

            return request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        public static void SetTokenCookie(string refreshToken, HttpResponse response)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
