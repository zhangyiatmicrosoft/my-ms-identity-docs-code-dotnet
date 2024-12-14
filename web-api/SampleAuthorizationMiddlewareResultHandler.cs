using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using System.IdentityModel.Tokens.Jwt;

public class SampleAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        // If the authorization was forbidden and the resource had a specific requirement,
        // provide a custom 404 response.
        //if (authorizeResult.Forbidden
        //    && authorizeResult.AuthorizationFailure!.FailedRequirements
        //        .OfType<Show404Requirement>().Any())
        //{
        //    // Return a 404 to make it appear as if the resource doesn't exist.
        //    context.Response.StatusCode = StatusCodes.Status404NotFound;
        //    return;
        //}

        var authHeader = context.Request.Headers["Authorization"];
        if (authHeader.Count == 1)
        {
            var authHeaderVal = authHeader[0];
            if (authHeaderVal == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
            if (authHeaderVal.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var rawTokenString = authHeaderVal.Substring("Bearer ".Length).Trim();
                var token = Decode(rawTokenString);
                var claims = token.Claims;
                foreach (var claim in claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type} Claim Value: {claim.Value}");
                }
            }
        }

        // Fall back to the default implementation.
        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }

    public static JwtSecurityToken Decode(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);
        return token;
    }
}

public class Show404Requirement : IAuthorizationRequirement { }