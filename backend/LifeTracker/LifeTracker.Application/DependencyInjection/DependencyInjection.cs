using FluentValidation;
using LifeTracker.Application.Interfaces.Services;
using LifeTracker.Application.Services;
using LifeTracker.Application.Validators.Todos;
using Microsoft.Extensions.DependencyInjection;

namespace LifeTracker.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateTodoValidator>();
        services.AddScoped<ITodoService, TodoService>();

        return services;
    }
}
