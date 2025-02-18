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
using WarehouseAPI.Controllers;
using WarehouseAPI.Messaging.Receive.Options;
using WarehouseAPI.Messaging.Receive.Receiver;
using WarehouseAPI.Service.Services;
using RabbitMQ.Client;


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
                            })
                            .AddApplicationPart(typeof(WarehouseController).Assembly); 


                        // Add Swagger 
                        services.AddEndpointsApiExplorer();
                        services.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Warehouse API", Version = "v1" });
                        });;

                        services.ConfigureSwaggerGen(options =>
                        {
                            options.AddEnumsWithValuesFixFilters();
                            options.DocInclusionPredicate((docName, apiDesc) =>
                            {
                                // Only include controllers in the desired namespace
                                return apiDesc.ActionDescriptor.RouteValues["controller"]?.StartsWith("Warehouse") ?? false;
                            });
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

                        //RabbitMQ
                        var serviceClientSettingsConfig = context.Configuration.GetSection("RabbitMq");
                        var serviceClientSettings = serviceClientSettingsConfig.Get<RabbitMqConfiguration>();

                        if (serviceClientSettings == null)
                        {
                            throw new InvalidOperationException("RabbitMq configuration is missing in appsettings.json.");
                        }
                        Console.WriteLine("RabbitMq configuration loaded successfully.");
                        
                        services.Configure<RabbitMqConfiguration>(serviceClientSettingsConfig);
                        services.AddHostedService<ProductBookedQuantityUpdateReceiver>();
                        services.AddSingleton<IProductBookedQuantityUpdateService, ProductBookedQuantityUpdateService>();

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