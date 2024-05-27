﻿using System.Reflection;
using Application.Interfaces;
using Application.Services.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Infra_Ioc.Application;

public static class EntitiesServicesDependecyInjection
{
    public static IServiceCollection AddEntitiesServicesDependecyInjection(this IServiceCollection services)
    {
        services.AddScoped<ICategoryDtoService, CategoryDtoService>();
        services.AddScoped<IProductDtoService, ProductDtoService>();
        services.AddScoped<IReviewDtoService, ReviewDtoService>();

        var applicationAssembly = AppDomain.CurrentDomain.Load("Application");
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(), applicationAssembly);
        });

        return services;
    }
}