using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // tells our AuctionService where to go to validate the token ( JWT )
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        // we are not returning audience with our token, so we're not able to validate audience
        options.TokenValidationParameters.ValidateAudience = false;
        // this is the custom claim we used that contains the name of the user
        options.TokenValidationParameters.NameClaimType = "username";
    });

var app = builder.Build();

app.MapReverseProxy();

app.Run();