using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Caroline.Api;
using Caroline.Areas.Api.Models;
using Caroline.Domain;
using Caroline.Models;
using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;

namespace Caroline.Areas.Api.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        public async Task<string> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonConvert.SerializeObject(new SuccessViewModel { Success = false, MissingField = true });
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return JsonConvert.SerializeObject(new SuccessViewModel { Success = true });
                case SignInStatus.LockedOut:
                    return JsonConvert.SerializeObject(new SuccessViewModel { Success = false, Lockout = true });
                case SignInStatus.RequiresVerification:
                    return JsonConvert.SerializeObject(new SuccessViewModel { Success = false });
                case SignInStatus.Failure:
                default:
                    return JsonConvert.SerializeObject(new SuccessViewModel { Success = false });
            }
        }

        // /hyper/seecret/adventure
        [Route("hyper/seecret/adventure")]
        public async Task<string> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return JsonConvert.SerializeObject(new SuccessViewModel { Success = false, MissingField = true });

            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
            if (!await AnonymousProfileApi.TryMigrateAnonymousAccountOrRegister(HttpContext, model))
                return JsonConvert.SerializeObject(new SuccessViewModel { Success = false });

            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return JsonConvert.SerializeObject(new SuccessViewModel { Success = true });
        }

        //public string ForgotPassword(string email)
        //{
        //    return null;
        //}

        public void LogOff() // ?? create anonymous user?
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
        }
    }
}