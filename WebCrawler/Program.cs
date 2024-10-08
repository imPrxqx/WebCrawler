using Microsoft.EntityFrameworkCore;
using WebCrawler.Controllers;
using WebCrawler.GraphQl;
using WebCrawler.Models;

namespace WebCrawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );
            builder
                .Services.AddGraphQLServer()
                .AddQueryType<Query>()
                .AddType<WebCrawler.GraphQl.WebPage>()
                .AddType<WebCrawler.GraphQl.Node>()
                .RegisterService<ApplicationDbContext>();

            //Add Swagger OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<RecordsQueue>();

            builder.Services.AddHostedService<BackgroundObserver>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.MapGraphQL();

            app.Run();
        }
    }
}
