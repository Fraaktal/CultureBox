using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CultureBox.Control;
using CultureBox.DAO;

namespace CultureBox
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerDocument();
            ConfigureDependency(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseOpenApi();
            app.UseSwaggerUi3();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

        private void ConfigureDependency(IServiceCollection services)
        {
            services.AddSingleton<IDbExecutor, DbExecutor>();
            services.AddScoped<IApiBookController, ApiBookController>();
            services.AddScoped<IApiMovieSerieController, ApiMovieSerieController>();
            services.AddScoped<IUserDAO, UserDAO>();
            services.AddScoped<IBookDAO, BookDAO>();
            services.AddScoped<IMovieDao, MovieDao>();
            services.AddScoped<ISeriesDao, SeriesDao>();
            services.AddScoped<ICollectionDAO, CollectionDAO>();
            services.AddScoped<ILoanRequestControllerDAO, LoanRequestControllerDAO>();
        }
    }
}
