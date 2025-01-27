using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using WarehouseAPI.Database;
using WarehouseAPI.Domain.Entities;
using WarehouseAPI.Models;
using WarehouseAPI.Service.Command;
using WarehouseAPI.Service.Query;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WarehouseAPI.Database.Repository;
using System.Text.Json;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using System.Text.Json.Serialization;
using OrderAPI;
using OrderAPI.Database;
using OrderAPI.Database.Repository;
using OrderAPI.Domain.Entities;
using OrderAPI.Infrastructure.AutoMapper;
using AutoMapper;

namespace WarehouseApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        // Add DbContext with PostgreSQL
                        services.AddDbContext<ProductContext>(options =>
                            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
                        // Add DbContext with PSQL of OrderAPI
                        services.AddDbContext<OrderContext>(options =>
                            options.UseNpgsql(context.Configuration.GetConnectionString("ConnectionToOrderAPI")));    

                        // Add MediatR for handling commands and queries
                        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

                        //services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
                        services.AddTransient<IProductRepository, ProductRepository>();
                        services.AddTransient<IOrderRepository, OrderRepository>();

                        //services.AddTransient<IValidator<CreateProductModel>, CreateProductModelValidator>();
                        //services.AddTransient<IValidator<UpdateProductModel>, UpdateProductModelValidator>();

                        // Other necessary services (e.g., controllers)
                        services.AddControllers()
                            .AddJsonOptions(options =>
                            {
                            // Apply JsonConverter globally for Enums to be serialized as strings
                                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                            });

                        // Add Swagger 
                        services.AddEndpointsApiExplorer();
                        services.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Warehouse API", Version = "v1" });
                        });;

                        services.ConfigureSwaggerGen(options =>
                        {
                            options.AddEnumsWithValuesFixFilters();
                        });

                        //AutoMapper
                        services.AddAutoMapper(typeof(Program));

                        var serviceProvider = services.BuildServiceProvider();
                        var mapper = serviceProvider.GetRequiredService<IMapper>();

                        try
                        {
                        mapper.ConfigurationProvider.AssertConfigurationIsValid();
                        Console.WriteLine("AutoMapper configuration is valid.");
                        }
                        catch (Exception ex)
                        {  
                            Console.WriteLine("AutoMapper configuration error: " + ex.Message);
                            throw;
                        }

                        services.AddScoped<PutOrderByIdQuery>();

                        //HTTP Client
                        var baseAddress = context.Configuration["OrderAPI:BaseAddress"];
                        services.AddHttpClient("OrderAPIClient", client =>
                        {
                            client.BaseAddress = new Uri(baseAddress);
                            client.DefaultRequestHeaders.Accept.Add(
                            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        });


                    })

                    .Configure(app =>
                    {
                        var environment = app.ApplicationServices.GetService<IHostEnvironment>();
             
                        if (environment.IsDevelopment())
                        {
                            app.UseSwagger(c =>
                        {
                            c.SerializeAsV2 = true;
                        });
                            app.UseSwaggerUI(options =>
                            {
                                options.SwaggerEndpoint("/swagger/v1/swagger.json", "WarehouseAPI v1");
                                options.RoutePrefix = "swagger"; // Swagger UI available at /swagger in production
                            });
                            app.UseDeveloperExceptionPage();
                        }
                        else
                        {
                            app.UseExceptionHandler("/Home/Error");
                            app.UseHsts();
                        }

                        app.UseHttpsRedirection();
                        app.UseStaticFiles();
                        app.UseRouting();
                        app.UseAuthorization();
                        app.UseDeveloperExceptionPage();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();  // This maps the controllers to endpoints
                            endpoints.MapGet("/", context =>
                            {
                                context.Response.Redirect("/swagger/index.html");
                                return Task.CompletedTask;
                            });
                        });

                        app.Use(async (context, next) =>
                            {
                                // Log request details
                                Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");

                                await next();

                                // Log response details
                                Console.WriteLine($"Response: {context.Response.StatusCode}");
                            });
                    });
                });
            
    }
}