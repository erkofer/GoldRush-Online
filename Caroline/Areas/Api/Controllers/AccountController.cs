using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Caroline.Api;
using Caroline.Areas.Api.Models;
using Caroline.Domain;
using Caroline.Extensions;
using Caroline.Models;
using Caroline.Persistence;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;

namespace Caroline.Areas.Api.Controllers
{
    public class AccountController : Controller
    {
        private UserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(UserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public UserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<UserManager>();
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
                return JsonConvert.SerializeObject(new IdentityResult());
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            IdentityResult ret;
            switch (result)
            {
                case SignInStatus.Success:
                    ret = IdentityResult.Success;
                    break;
                case SignInStatus.LockedOut:
                    ret = new IdentityResult("Locked out.");
                    break;
                case SignInStatus.RequiresVerification:
                    ret = new IdentityResult("Your email address must be verified.");
                    break;
                case SignInStatus.Failure:
                default:
                    ret = new IdentityResult("We couldn't sign you in, sure you have the right credentials?");
                    break;
            }
            return JsonConvert.SerializeObject(ret);
        }

        // /hyper/seecret/adventure
        //[Route("hyper/seecret/adventure")]
        public async Task<string> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return JsonConvert.SerializeObject(new IdentityResult(ModelState.GetErrors()));

            var result = await AnonymousProfileApi.TryMigrateAnonymousAccountOrRegister(HttpContext, model);
            return JsonConvert.SerializeObject(result);

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
        }

        //public string ForgotPassword(string email)
        //{
        //    return null;
        //}

        public async Task<string> Info()
        {
            var userId = HttpContext.User.Identity.GetUserId<long>();
            var account = new AccountViewModel();

            var db = await CarolineRedisDb.CreateAsync();
            var user = await db.Users.Get(userId);


            if (user == null)
                account.SignedIn = false;
            else
            {
                account.SignedIn = true;
                if (user.IsAnonymous)
                    account.Anonymous = true;
                else
                    account.UserName = user.UserName;
            }
            return JsonConvert.SerializeObject(account);
        }

        public void LogOff() // ?? create anonymous user?
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
        }
    }
}