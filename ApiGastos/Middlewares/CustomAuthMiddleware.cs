using ApiGastos.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGastos.Middlewares
{
    public class CustomAuthMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly AppDbContext DbContext;

        public CustomAuthMiddleware(AppDbContext AppDbContext)
        {
            this.DbContext = AppDbContext;
        }
        public CustomAuthMiddleware(RequestDelegate next)
        {
            
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            string authHeader = httpContext.Request.Headers["Authorization"];
            if (authHeader != null)
            {
                string auth = authHeader.Split(new char[] { ' ' })[1];
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                var tokeAppSolicitud = encoding.GetString(Convert.FromBase64String(auth));
                string app = tokeAppSolicitud.Split(new char[] { ':' })[0];
                string token = tokeAppSolicitud.Split(new char[] { ':' })[1];

                var validacionToken = DbContext.Token.Where(f => f.apikey == token && f.aplicacion == app).FirstOrDefault();

                if (validacionToken != null)
                {
                    await _next(httpContext);
                }
                else
                {
                    httpContext.Response.StatusCode = 401;
                    return;
                }
            }
            else
            {
                httpContext.Response.StatusCode = 401;
                return;
            }
        }

    }
    public static class BasicAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiGastos.Middlewares.CustomAuthMiddleware>();
        }
    }
}
