﻿using AutoMapper;
using Api.Attributes;
using Api.Dto.Autommaper;
using Api.Dto.Base;
using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Api.Net.Core.DataAccess.Entity;
using Api.Net.Core.Utils;

namespace Api.Utils
{
    public static class MapperUtils
    {
        public static void ResolveDtoMaps(this IMapperConfigurationExpression mapper)
        {
            var types = GetAllDtos();
            foreach (var type in types)
            {
                var dto = Activator.CreateInstance(type);
                var interfaces = type.GetInterfaces().Where(t2 => t2.IsGenericType && t2.GetGenericTypeDefinition() == typeof(IDto<,>));
                foreach (var @interface in interfaces)
                {
                    var genericBackArguments = @interface.GetGenericArguments();
                    var genericArguments = genericBackArguments.Reverse().ToArray();
                    if (genericArguments.Length < 2) continue;

                    var createMaps = mapper.GetType().GetMethods().First(t => t.Name == "CreateMap" && t.IsGenericMethod);
                    var map = createMaps.MakeGenericMethod(genericArguments).Invoke(mapper, null);
                    var mapConfig = Activator.CreateInstance(typeof(Maps<,>).MakeGenericType(genericBackArguments), map);
                    type.GetMethod(nameof(IDto<object, object>.MapExpandables)).Invoke(dto, new[] { mapConfig });
                    type.GetMethod(nameof(IDto<object, object>.Map)).Invoke(dto, new[] { mapConfig });


                    var mapBack = createMaps.MakeGenericMethod(genericBackArguments).Invoke(mapper, null);
                    var mapBackConfig = Activator.CreateInstance(typeof(Maps<,>).MakeGenericType(genericArguments), mapBack);
                    type.GetMethod(nameof(IDto<object, object>.MapBack)).Invoke(dto, new[] { mapBackConfig });
                }
            }
        }

        public static IEnumerable<Type> GetAllDtos()
        {

            var types = AssemblyUtils.GetAssemblies().SelectMany(t => t.DefinedTypes).Where(t => !t.IsAbstract);
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces().Where(t2 => t2.IsGenericType && t2.GetGenericTypeDefinition() == typeof(IDto<,>));
                if (!interfaces.Any()) continue;
                yield return type;
            }
        }
        public static IEnumerable<ContextEntities> GetAllContextEntities()
        {
            return GetContextEntities().Concat(GetBaseEntities());
        }

        private static IEnumerable<ContextEntities> GetContextEntities()
        {
            var asseblies = AssemblyUtils.GetAssemblies();
            var contexts = asseblies.SelectMany(t => t.DefinedTypes).Where(t => !t.IsAbstract && t.InheritsFrom(typeof(DbContext)));

            foreach (var context in contexts)
            {
                var entities = context.GetProperties().Where(t => t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                                       .SelectMany(t => t.PropertyType.GetGenericArguments());
                foreach (var entity in entities)
                    yield return new ContextEntities { DbContextType = context, EntityType = entity };
            }
        }

        private static IEnumerable<ContextEntities> GetBaseEntities()
        {
            List<Type> entities = new List<Type>();
            var types = AssemblyUtils.GetAssemblies().SelectMany(t => t.DefinedTypes).Where(t => !t.IsAbstract);
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces().Where(t2 => t2.IsGenericType && t2.GetGenericTypeDefinition() == typeof(IBaseEntity<,>));
                if (!interfaces.Any()) continue;
                entities.Add(interfaces.FirstOrDefault());

            }
            foreach (var entity in entities)
                yield return new ContextEntities { DbContextType = entity.GetGenericArguments()[1], EntityType = entity.GetGenericArguments()[0] };
        }
        public static IEnumerable<Type> GetAllContext()
        {
            var assemblies = AssemblyUtils.GetAssemblies();

            var contexts = assemblies.SelectMany(t => t.DefinedTypes).Where(t => !t.IsAbstract && t.InheritsFrom(typeof(DbContext)));
            return contexts;
        }
        public static IEnumerable<Type> GetApiRepositories()
        {
            var types = AssemblyUtils.GetAssemblies().SelectMany(t => t.DefinedTypes)
                                  .Where(t => t.GetCustomAttribute<ApiRepository>() != null);
            return types;
        }
        public static IEnumerable<Type> GetApiServices()
        {
            var types = AssemblyUtils.GetAssemblies().SelectMany(t => t.DefinedTypes)
                                  .Where(t => t.GetCustomAttribute<ApiService>() != null);
            return types;
        }
    }
}