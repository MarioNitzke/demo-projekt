using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Cortex.Mediator.Queries;
using PhysioBook.Auth;
using Stripe;
using PhysioBook.Data;
using PhysioBook.Data.Entities;
using PhysioBook.Exceptions;
using PhysioBook.Extensions;

namespace PhysioBook.Configurations;

public static class ServiceRegistrations
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        // Exception handler
        builder.Services.AddExceptionHandler<GlobalExceptionHandlerMiddleware>();
        builder.Services.AddProblemDetails();

        // DbContext with IDbContextFactory
        builder.Services.AddDbContextFactory<PhysioBookContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddDbContext<PhysioBookContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Identity
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<PhysioBookContext>()
            .AddDefaultTokenProviders();

        // JWT Settings
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtAuthSettings>()!;
        builder.Services.Configure<JwtAuthSettings>(configuration.GetSection("JwtSettings"));

        // Authentication
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        // Authorization policies
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(AppPolicies.Visitor, policy => policy.RequireAssertion(_ => true))
            .AddPolicy(AppPolicies.Admin, policy => policy.RequireRole(AppRoles.Admin));

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // Stripe
        var stripeSettings = configuration.GetSection("Stripe").Get<StripeSettings>()!;
        builder.Services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
        StripeConfiguration.ApiKey = stripeSettings.SecretKey;

        // Cortex.Mediator
        builder.Services.AddCortexMediator(builder.Configuration, new[] { typeof(Program) });

        // ValidationBehavior pipeline
        builder.Services.AddTransient(typeof(IQueryPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // FluentValidation validators
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        // Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PhysioBook API",
                Version = "v1",
                Description = "PhysioBook REST API"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}
