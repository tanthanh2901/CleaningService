//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;

//namespace CatalogService.Extensions
//{
//    public static class CorsExtensions
//    {
//        public static IServiceCollection AddCorsServices(this IServiceCollection services)
//        {
//            services.AddCors(options =>
//            {
//                options.AddPolicy("ReactAppPolicy", builder =>
//                {
//                    builder.WithOrigins("http://localhost:5173")
//                           .AllowAnyMethod()
//                           .AllowAnyHeader()
//                           .AllowCredentials();
//                });
//            });

//            return services;
//        }

//        public static IApplicationBuilder UseCorsServices(this IApplicationBuilder app)
//        {
//            app.UseCors("ReactAppPolicy");
//            return app;
//        }
//    }
//} 