using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Appets.BusinessLogic.Interface;
using Appets.Exceptions;

namespace Appets.WebApi.Filters
{
    public class AuthenticationFilter : Attribute, IAuthorizationFilter
    {
        IAuthenticationLogic AuthenticationLogic;
        public AuthenticationFilter(IAuthenticationLogic authenticationLogic)
        {
            this.AuthenticationLogic = authenticationLogic;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string token = context.HttpContext.Request.Headers["auth"];
            if (token == null)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "No access token found on the request"
                };
            }
            else
            {
                try
                {
                    Guid guidToken = Guid.Parse(token);
                    if (!this.AuthenticationLogic.IsLoggedIn(guidToken))
                    {
                        context.Result = new ContentResult()
                        {
                            StatusCode = 401,
                            Content = "User not authorized to use the service, try to login first"
                        };
                    }
                }
                catch (BadLoginException)
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = 401,
                        Content = "User not authorized to use the service, try to login first"
                    };
                }
                catch (Exception)
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = 500,
                        Content = "Something went wrong on the server"
                    };
                }
            }
        }
    }
}
