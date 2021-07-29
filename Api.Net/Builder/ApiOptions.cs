using Api.Net.Core.Conventions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Api.Builder
{
    public delegate DbContextOptionsBuilder<TContext> DbContextConfig<TContext>(DbContextOptionsBuilder<TContext> builder) where TContext: DbContext;

    public class ApiOptions
    {
        private readonly Dictionary<Type, Func<DbContextOptions>> ContextOptionsProvider = new Dictionary<Type, Func<DbContextOptions>>();

        public string RoutePrefix { get; set; } = "api";

        public ApiConvention Conventions { get; } = new ApiConvention();

        public void UseDbContext<TContext>() where TContext: DbContext
        {
            UseDbContext<TContext>(_ => _);
        }

        public void UseDbContext<TContext>(DbContextConfig<TContext> config) where TContext : DbContext
        {
            var contextType = typeof(TContext);
            var builder = new DbContextOptionsBuilder<TContext>();
            ContextOptionsProvider.Add(contextType, () => config(builder).UseLazyLoadingProxies().Options);
        }

        internal DbContextOptions GetDbContextOptions(Type contextType)
        {
            if (ContextOptionsProvider.TryGetValue(contextType, out Func<DbContextOptions> factory))
            {
                return factory();
            }

            return null;
        }
    }
}