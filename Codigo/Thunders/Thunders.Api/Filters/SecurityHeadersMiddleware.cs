using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Thunders.Api.Filters
{
    public class SecurityHeadersMiddleware(RequestDelegate _next)
    {
        public async Task Invoke(HttpContext context)
        {
            HandleHeaders(context);
            await _next(context);
        }

        private static void HandleHeaders(HttpContext context)
        {
            context.Response.Headers.Append("X-Frame-Options", "Deny");
            context.Response.Headers.Append("Referrer-Policy", "origin");
            context.Response.Headers.Append("Permissions-Policy", "geolocation=(), midi=(), camera=(),usb=(), magnetometer=(), sync-xhr=(), microphone=(), camera=(), gyroscope=(), speaker=(), payment=()");
            //#if DEBUG
            //            // SECURE: Enable Content security policy with reporting
            //            context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; style-src 'self' 'unsafe-inline' https://unpkg.com/gridjs/; img-src * data:; font-src 'self' https: data:; script-src 'self' https://unpkg.com/gridjs-jquery/; connect-src 'self'; frame-ancestors 'self'; form-action 'self'; base-uri 'self'; object-src 'none'; report-uri /Security/CspReporting");
            //#else
            //			context.Response.Headers.Add("Content-Security-Policy", "default-src https:; style-src https: 'unsafe-inline' https://unpkg.com/gridjs/; img-src https: data:; font-src https: data:; script-src https: https://unpkg.com/gridjs-jquery/; connect-src https:; frame-ancestors https:; form-action https:; base-uri https:; object-src 'none'; report-uri /Security/CspReporting");
            //#endif
            // SECURE: Enable Expect-CT header with reporting
            //context.Response.Headers.Add("Expect-CT", "max-age=0, report-uri /Security/CtReporting");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff"); // SECURE: Prevent site being displayed in a different format in older browsers c.f. https://www.owasp.org/index.php/List_of_useful_HTTP_headers
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block; report=/Security/CspReporting"); // SECURE: Enable browsers anti-XSS protection and report any violations  https://www.owasp.org/index.php/XSS_(Cross_Site_Scripting)_Prevention_Cheat_Sheet
                                                                                                                 // SECURE: Public key pinning to be deprecated, do not use: http://www.zdnet.com/article/google-chrome-is-backing-away-from-public-key-pinning-and-heres-why/
            context.Response.Headers.Append("Public-Key-Pins", "max-age=0; includeSubDomains");
            // Remove public key pinning 
            //#if DEBUG
            //            context.Response.Headers.Add("Strict-Transport-Security", "max-age=0; includeSubDomains"); // Remove HSTS header for debug
            //#else
            //// SECURE: Enforce any further requests after initial request to be made over TLS, register for HSTS pre-load
            //			context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
            //#endif
            context.Response.Headers.Remove("Server"); // SECURE: Remove server information disclosure
        }
    }
}
