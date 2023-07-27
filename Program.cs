using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalChatAppApi.Data;
using MinimalChatAppApi.Interfaces;
using MinimalChatAppApi.Middlewares;
using MinimalChatAppApi.Models;
using MinimalChatAppApi.Repositories;
using MinimalChatAppApi.Services;
using System.Text;

namespace MinimalChatAppApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
         
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //added cors
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            builder.Services.AddDbContextFactory<ChatContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("ChatContext")));
            builder.Services.AddDbContext<ChatContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("ChatContext")));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ChatContext>().AddDefaultTokenProviders();
            
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = "JWTServicePostmanClient",
                    ValidIssuer = "JWTAuthenticationServer",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]))
                };
            });

            //builder.Services.AddScoped<IRepository<Message>, Repository<Message>>();
            //builder.Services.AddScoped<IRepository<User>, Repository<User>>();
            builder.Services.AddScoped<IRepository<LogModel>, Repository<LogModel>>();
            //builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<ILogService, LogService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
           
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<ChatContext>();
                context.Database.EnsureCreated();
                // DbInitializer.Initialize(context);
            }

        
            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(); // Enable CORS
            app.MapControllers();
            app.UseMiddleware<LoggingMiddleware>();
            app.Run();
        }
    }
}