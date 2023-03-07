using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Web.Http.ModelBinding;
using Ivap.Models;
using Ivap.ViewModel;
using Ivap.Repository;

namespace Ivap.ActionFilters
{
    
    public class AuthorizeApi : AuthorizationFilterAttribute
    {


        public override void OnAuthorization(HttpActionContext actionContext)
        {
            
            IEnumerable<string> ApiKeyHeaderUID = null;
            IEnumerable<string> ApiKeyHeaderPWD = null;
            if (actionContext.Request.Headers.TryGetValues("UserName", out ApiKeyHeaderUID) && actionContext.Request.Headers.TryGetValues("Password", out ApiKeyHeaderPWD))
            {
                string UserName = ApiKeyHeaderUID.First().ToString();
                string PWD = ApiKeyHeaderPWD.First().ToString();
                // Validating header value must have both APP ID & APP key
                if (UserName == "" || PWD == "")
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "UserName or Password Missing!.");
                    return;
                }
                if (UserName != "" && PWD != "")
                {
                    AccountRepo ARepo = new AccountRepo();
                    LoginVM Model = new LoginVM();
                    Model.UserName = UserName;
                    Model.Password = PWD;
                    AppUser OUser = new AppUser();
                    string Message = "";
                    OUser = ARepo.Authenticate(Model, ref Message);
                    AccountRepo ObjURep = new AccountRepo();
                    ObjURep.LogLogin(OUser.UID, "Mobile-Login");
                    if (Message == "success")
                    {
                        IList<Claim> claimCollection = new List<Claim>
                        {
                           new Claim("UID", OUser.UID.ToString()),
                           new Claim("UserID", OUser.UserID),
                           new Claim("FirstName", OUser.FirstName),
                           new Claim("LastName", OUser.LastName),
                           new Claim("Email", OUser.Email),
                           new Claim("Role", OUser.Role.ToString()),
                           new Claim("RoleName", OUser.RoleName),
                           new Claim("PassChangeDays", OUser.PassChangeDays.ToString()),
                        };

                        var identity = new ClaimsIdentity(claimCollection);
                        var principal = new ClaimsPrincipal(identity);
                        Thread.CurrentPrincipal = principal;
                        if (System.Web.HttpContext.Current != null)
                        {
                            System.Web.HttpContext.Current.User = principal;
                        }
                    }
                    else { actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid UserName or Password !"); }
                }

            }

            else { actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "UserName or Password Missing!."); }


            base.OnAuthorization(actionContext);

        }



    }
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        public BasicAuthenticationIdentity(string name, string password)
            : base(name, "Basic")
        {
            this.Password = password;
        }

        /// <summary>
        /// Basic Auth Password for custom authentication
        /// </summary>
        public string Password { get; set; }
    }
}