
//var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using OrderAPI.Database;
using OrderAPI.Domain.Entities;
using Microsoft.Extensions.Logging;
//using OrderApi.Infrastructure.Prometheus;
//using OrderApi.Messaging.Send.Options.v1;
//using OrderApi.Messaging.Send.Sender.v1;
using OrderAPI.Models;
using OrderAPI.Service.Command;
using OrderAPI.Service.Query;
//using OrderApi.Validators.v1;
//using FluentValidation;
//using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OrderAPI.Database.Repository;
using AutoMapper;
using System.Text.Json;
//using Prometheus;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using System.Text.Json.Serialization;
using OrderAPI.Messaging.Send.Sender;
using OrderAPI.Messaging.Send.Options;
using RabbitMQ.Client;

namespace OrderAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); // Clears default logging providers
                    logging.AddConsole();     // Adds console logging
                    logging.SetMinimumLevel(LogLevel.Information); // Ensures logs show up
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        // Add DbContext with PostgreSQL
                        services.AddDbContext<OrderContext>(options =>
                            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));

                        // Add MediatR for handling commands and queries
                        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

                        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
                        services.AddTransient<IOrderRepository, OrderRepository>();

                        //services.AddTransient<IValidator<CreateOrderModel>, CreateOrderModelValidator>();
                        //services.AddTransient<IValidator<UpdateOrderModel>, UpdateOrderModelValidator>();

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
                            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order API", Version = "v1" });
                        });;

                        services.ConfigureSwaggerGen(options =>
                        {
                            options.AddEnumsWithValuesFixFilters();
                        });

                        //AutoMapper
                        services.AddAutoMapper(typeof(Program));

                        //RabbitMQ
                        var serviceClientSettingsConfig = context.Configuration.GetSection("RabbitMq");
                        var serviceClientSettings = serviceClientSettingsConfig.Get<RabbitMqConfiguration>();

                        if (serviceClientSettings == null)
                        {
                            throw new InvalidOperationException("RabbitMq configuration is missing in appsettings.json.");
                        }
                        Console.WriteLine("RabbitMq configuration loaded successfully.");
                        
                        services.Configure<RabbitMqConfiguration>(serviceClientSettingsConfig);

                        services.AddSingleton<IOrderUpdateSender, OrderUpdateSender>();

                        // Register RabbitMQ connection
                        services.AddSingleton<IConnection>(sp =>
                        {
                            var logger = sp.GetRequiredService<ILogger<Program>>();
                            try
                            {
                                var factory = new ConnectionFactory
                                {
                                    HostName = serviceClientSettings.Hostname,
                                    UserName = serviceClientSettings.UserName,
                                    Password = serviceClientSettings.Password,
                                    //VirtualHost = serviceClientSettings.VirtualHost,
                                    //DispatchConsumersAsync = true
                                };

                                // Lazy initialization of connection to ensure it's created only when needed
                                var connection = new Lazy<Task<IConnection>>(() =>  factory.CreateConnectionAsync());
                                logger.LogInformation("RabbitMQ connection created successfully.");
                                Console.WriteLine("RabbitMQ connection created successfully.");
                                return connection.Value.Result; // Block here to resolve the connection synchronously
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Failed to create RabbitMQ connection.");
                                Console.WriteLine("RabbitMQ connection failed.");
                                throw; // Re-throw the exception to fail the startup if the connection cannot be created
                            }
                           
                        });

                        // Register RabbitMQ channel
                        services.AddSingleton<IChannel>(sp =>
                        {
                            var logger = sp.GetRequiredService<ILogger<Program>>();
                            try 
                            {
                            var connection = sp.GetRequiredService<IConnection>();

                            // Asynchronously create the channel and wait for the result
                            var channel = new Lazy<Task<IChannel>>(() => connection.CreateChannelAsync()); // Wait synchronously for the async result
                            logger.LogInformation("RabbitMQ channel created successfully.");
                            return channel.Value.Result;
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Failed to create RabbitMQ channel.");
                                throw; 
                            }
                           
                            

                        });
                        
                        

                        
                        webBuilder.UseUrls("http://localhost:6020", "http://0.0.0.0:6020");

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
                                options.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderAPI v1");
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
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();  // This maps the controllers to endpoints
                                    // Redirect root to Swagger UI
                            endpoints.MapGet("/", context =>
                            {
                                context.Response.Redirect("/swagger/index.html");
                                return Task.CompletedTask;
                            });
                        });
                    });
            
                });
    }
}   

// end new code

//var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}



//app.UseHttpsRedirection();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast =  Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast")
//.WithOpenApi();

//app.Run();

//record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}
