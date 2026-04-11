using System.Data;
using Microsoft.OpenApi.Models;
using Npgsql;
using PhysioBook.Api.Extensions;

namespace PhysioBook.Api.Configurations;

public static class ServiceRegistrations
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.Configure<JwtAuthSettings>(configuration.GetSection(JwtAuthSettings.SectionName));

        services.AddHttpContextAccessor();
        services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed(_ => true);
            });
        });

        services.AddScoped<IDbConnection>(_ =>
            new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));

        services.AddDbContextFactory<PhysioBookContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<PhysioBookContext>()
            .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection(JwtAuthSettings.SectionName).Get<JwtAuthSettings>() ?? new JwtAuthSettings();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AppPolicies.Visitor, policy => policy.RequireAssertion(_ => true));
            options.AddPolicy(AppPolicies.Admin, policy => policy.RequireRole(AppRoles.Admin));
        });

        services.AddCortexMediator(builder.Configuration, new[] { typeof(Program) });
        services.AddTransient(typeof(IQueryPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddHealthChecks().AddDbContextCheck<PhysioBookContext>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PhysioBook API",
                Version = "v1",
                Description = "Demo scaffold for PhysioBook reservation system."
            });

            var bearerSecurityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Input a valid JWT token."
            };

            options.AddSecurityDefinition("Bearer", bearerSecurityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    bearerSecurityScheme,
                    Array.Empty<string>()
                }
            });
        });
    }
}
