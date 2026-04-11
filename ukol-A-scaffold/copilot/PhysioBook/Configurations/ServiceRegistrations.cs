using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace PhysioBook.Configurations;

public static class ServiceRegistrations
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtAuthSettings>(builder.Configuration.GetSection(JwtAuthSettings.SectionName));


        builder.Services.AddDbContextFactory<PhysioBookContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentityCore<AppUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<PhysioBookContext>()
            .AddSignInManager();

        var jwtSettings = builder.Configuration.GetSection(JwtAuthSettings.SectionName).Get<JwtAuthSettings>()
                          ?? throw new InvalidOperationException("JWT settings are missing.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));

        builder.Services
            .AddAuthentication(options =>
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
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(AppPolicies.Visitor, policy => policy.RequireAssertion(_ => true));
            options.AddPolicy(AppPolicies.Admin, policy => policy.RequireRole(AppRoles.Admin));
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddCortexMediator(builder.Configuration, new[] { typeof(Program) });
        RegisterValidators(builder.Services);
        builder.Services.AddTransient(typeof(IQueryPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    private static void RegisterValidators(IServiceCollection services)
    {
        var validatorInterface = typeof(IValidator<>);
        var assembly = Assembly.GetExecutingAssembly();

        var validatorTypes = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorInterface)
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var validator in validatorTypes)
        {
            services.AddTransient(validator.Interface, validator.Implementation);
        }
    }
}

