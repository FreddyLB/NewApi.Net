using Api.Net.Core.Conventions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Api.Builder
{
    public delegate DbContextOptionsBuilder<TContext> DbContextConfig<TContext>(DbContextOptionsBuilder<TContext> builder) where TContext: DbContext;

    public class ApiOptions
    {
        public ApiOptions()
        {
            Conventions = new ApiConvention();
        }

        public string RoutePrefix { get; set; } = "api";

        // Breaking change
        // public Func<DbContextOptionsBuilder, string, DbContextOptionsBuilder> ContextOption { get; set; }

        public Func<DbContextOptionsBuilder, DbContextOptionsBuilder> ContextOption { get; set; }

        public ApiConvention Conventions { get; }

        private readonly Dictionary<Type, DbContextOptions> ContextOptions = new Dictionary<Type, DbContextOptions>();

        public void UseDbContext<TContext>() where TContext: DbContext
        {
            UseDbContext<TContext>(_ => _);
        }

        public void UseDbContext<TContext>(DbContextConfig<TContext> config) where TContext : DbContext
        {
            var contextType = typeof(TContext);
            var builder = new DbContextOptionsBuilder<TContext>();
            var options = config(builder).UseLazyLoadingProxies().Options;
            ContextOptions.Add(contextType, options);
        }

        internal DbContextOptions GetDbContextOptions(Type contextType)
        {
            return ContextOptions[contextType];
        }
    }
}