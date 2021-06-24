using System;
using System.Linq;
using System.Reactive.Concurrency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Bang.Data;
using Bang.Pages;
using MatBlazor;
using Microsoft.AspNetCore.ResponseCompression;
using ReactiveUI;
using Refit;
using Rocket.Surgery.Airframe.Data.DuckDuckGo;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Bang
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var wasmScheduler = new WasmScheduler();
            RxApp.MainThreadScheduler = wasmScheduler;
            RxApp.TaskpoolScheduler = wasmScheduler;
            
            services.AddSignalR();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddMatBlazor();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            
            services
                .AddRefitClient<IDuckDuckGoApiClient>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://api.duckduckgo.com"));

            services
                .AddSingleton<WeatherForecastService>()
                .AddTransient<IDuckDuckGoService, DuckDuckGoService>()
                .AddTransient<SearchViewModel>()
                .AddTransient<CardViewModel>()
                .UseMicrosoftDependencyResolver();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}