using System;
using System.Linq;
using System.Reflection;
using FluxMediator.Core.Abstractions;
using FluxMediator.Core.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace FluxMediator.Core.Extensions
{
    /// <summary>
    /// Extensions for configuring MyMediator in the DI container
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MyMediator to the services with flexible assembly registration options:
        /// - No arguments: scans all assemblies in the current AppDomain
        /// - Assembly[]: scans only the specified assemblies
        /// - string[]: scans assemblies whose names start with any of the specified prefixes
        /// - Type[]: scans assemblies containing any of the specified types
        /// </summary>
        public static IServiceCollection AddMyMediator(this IServiceCollection services, params object[] args)
        {
            var assemblies = ResolveAssemblies(args);

            services.AddSingleton<IMediator, Mediator>();

            // Register command handlers
            RegisterHandlers(services, assemblies, typeof(IRequestHandler<,>));

            // Register query handlers
            RegisterHandlers(services, assemblies, typeof(IQueryHandler<,>));

            // Register notification handlers
            RegisterHandlers(services, assemblies, typeof(INotificationHandler<>));

            return services;
        }

        /// <summary>
        /// Resolves assemblies based on the provided arguments
        /// </summary>
        private static Assembly[] ResolveAssemblies(object[] args)
        {
            // Return ALL assemblies in the AppDomain if no args provided
            if (args.Length == 0)
            {
                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.FullName))
                    .ToArray();
            }

            // Return all assemblies explicitly provided
            if (args.All(a => a is Assembly))
                return args.Cast<Assembly>().ToArray();

            // Return assemblies filtered by namespace prefix
            if (args.All(a => a is string))
            {
                var prefixes = args.Cast<string>().ToArray();
                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a =>
                        !a.IsDynamic &&
                        !string.IsNullOrWhiteSpace(a.FullName) &&
                        prefixes.Any(p => a.FullName.StartsWith(p)))
                    .ToArray();
            }

            if (!args.All(a => a is Type))
                throw new ArgumentException(
                    "Invalid parameters for AddMyMediator(). Use: no arguments, Assembly[], string prefix[], or Type[].");

            // Return assemblies containing specific types (additional flexibility)
            var types = args.Cast<Type>().ToArray();
            return types.Select(t => t.Assembly).Distinct().ToArray();
        }

        /// <summary>
        /// Registers handlers of a specific interface type from the specified assemblies
        /// </summary>
        private static void RegisterHandlers(IServiceCollection services, Assembly[] assemblies,
            Type handlerInterfaceType)
        {
            var handlers = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract &&
                            t.GetInterfaces().Any(i =>
                                i.IsGenericType &&
                                i.GetGenericTypeDefinition() == handlerInterfaceType))
                .ToList();

            foreach (var handler in handlers)
            {
                var handlerInterfaces = handler.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType);

                foreach (var handlerInterface in handlerInterfaces)
                {
                    services.AddTransient(handlerInterface, handler);
                }
            }
        }
    }
}