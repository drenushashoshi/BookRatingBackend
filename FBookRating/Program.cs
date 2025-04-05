using FBookRating.Services.IServices;
using FBookRating.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Business_Logic_Layer;
using Business_Logic_Layer.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// -M Register db and Identity
builder.Services.AddBusinessServices(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = "https://lab1.com/roles"
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var claimsPrincipal = context.Principal;

            // Extract claims using the names from your Auth0 configuration
            var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value; // typically the 'sub' claim
            var userName = claimsPrincipal.FindFirst("https://lab1.com/username")?.Value;
            // Optionally, you might use the "nickname" or "name" claim for DisplayName
            var displayName = claimsPrincipal.FindFirst("nickname")?.Value ?? userName;
            var email = claimsPrincipal.FindFirst("https://lab1.com/email")?.Value;
            var profilePictureUrl = claimsPrincipal.FindFirst("picture")?.Value;

            // Resolve your user service from the dependency injection container
            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

            // Create or update the user record in your database
            await userService.CreateOrUpdateUserAsync(userId, userName, displayName, email, profilePictureUrl);
        },
        OnAuthenticationFailed = context =>
        {
            // Optionally handle authentication failures here
            return Task.CompletedTask;
        }
    };


});


//builder.Services.AddAuthorization();

//TEST
/**
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
**/





builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IReviewRatingService, ReviewRatingService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddHttpClient<IImageUploadService, ImageUploadService>();
builder.Services.AddScoped<IUserService, UserService>();

//builder.Services.AddTransient<IEmailSender<ApplicationUser>, NullEmailSender>();
//builder.Services.AddScoped<ITagService, TagService>();




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add security definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer abc123xyz\""
    });

    // Add security requirement
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
            new string[] {}
        }
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()   // Allow requests from any origin
                  .AllowAnyMethod()   // Allow any HTTP method (GET, POST, PUT, DELETE, etc.)
                  .AllowAnyHeader();  // Allow any headers
        });
});






var app = builder.Build();
app.UseCors("AllowAll");


//TEST
/**
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);
}
**/


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapIdentityApi<ApplicationUser>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();
