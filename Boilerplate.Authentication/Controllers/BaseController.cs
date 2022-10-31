using Boilerplate.Authentication.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Authentication.Controllers
{
    [Controller]
    public abstract class BaseController : ControllerBase
    {
        // returns the current authenticated account (null if not logged in)
        public Account Account => HttpContext.Items["Account"] as Account;

        public object MessageResponse(string message) => new { message = message };
    }
}