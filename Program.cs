using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("Bearer").AddJwtBearer();
//builder.Services.AddAuthorizationBuilder();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AuthZPolicy", policy => policy
        .RequireClaim("scope", "user:read")
        .RequireAuthenticatedUser()
        .RequireRole("admin")
        );

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// dotnet user-jwts create --audience "ecr-aud"
app.MapGet("/secret", (ClaimsPrincipal user) => $"Hello {user.Identity?.Name}. My secret")
    .RequireAuthorization();

// dotnet user-jwts create --audience "ecr-aud" --scope "user:read"
app.MapGet("/secret2", () => "This require the scope user:read!")
    .RequireAuthorization(p => p.RequireClaim("scope", "user:read"));

// dotnet user-jwts create --audience "ecr-aud" --role "admin"
string[] roles = new string[] { "admin" };
app.MapGet("/secret3", () => "This require the role: admin!")
    .RequireAuthorization(p => p.RequireRole(roles));

// dotnet user-jwts create --audience "ecr-aud" --scope "user:read" --role "admin"
app.MapGet("/secret4", () => "Policy")
    .RequireAuthorization("AuthZPolicy");

app.Run();
