// <ms_docref_import_types>
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
// </ms_docref_import_types>

// <ms_docref_add_msal>



WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//var config = builder.Configuration.GetSection("AzureAd");

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//           .AddMicrosoftIdentityWebApi(config, JwtBearerDefaults.AuthenticationScheme, true);

// policyBuilder.Requirements.Add(new ScopeAuthorizationRequirement() { RequiredScopesConfigurationKey = $"AzureAd:Scopes" });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
    // .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddSingleton<
    IAuthorizationMiddlewareResultHandler, SampleAuthorizationMiddlewareResultHandler>();

builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("AuthZPolicy",
        policyBuilder =>
        {
            policyBuilder.RequireAssertion(context =>
            {
                var allClaims = context.User.Claims;
                var length = allClaims.Count();
                var claimsList = allClaims.ToList();

                var identities = context.User.Identities;
                var length2 = identities.Count();
                var identitiesList = identities.ToList();

                //     var valid = context.User.HasClaim(
                //    c =>
                //(c.Type == "BadgeId" || c.Type == "TemporaryBadgeId")
                //&& c.Issuer == "https://microsoftsecurity");
                return true;
            });
        });
});


// </ms_docref_add_msal>

// <ms_docref_enable_authz_capabilities>
WebApplication app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
// </ms_docref_enable_authz_capabilities>

var weatherSummaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// <ms_docref_protect_endpoint>
app.MapGet("/weatherforecast", [Authorize(Policy = "AuthZPolicy")] () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           weatherSummaries[Random.Shared.Next(weatherSummaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/", () =>
{
    var content = DateTime.Now.ToString();
    return content;
})
.WithName("Root");

app.MapGet("/weatherforecast2", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           weatherSummaries[Random.Shared.Next(weatherSummaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast2");

// </ms_docref_protect_endpoint>

app.Run();

record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
