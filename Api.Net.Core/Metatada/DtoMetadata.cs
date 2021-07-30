using Api.Net.Core.Conventions;
using Api.Net.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Api.Net.Core.Metatada
{
    public class DtoMetadata
    {
        public static DtoMetadata Instance { get; } = new DtoMetadata();

        private DtoMetadata()
        {
            Projections = new Dictionary<Type, IEnumerable<ProjectionDefinition>>();
        }

        public Dictionary<Type, IEnumerable<ProjectionDefinition>> Projections { get; set; }

        #warning This is not null safe, will be null if `AddApi` is not called
        public ApiConvention Convention { get; set; } = default!;

        public ProjectionDefinition? ResolveProyection(Type dtoType, string name)
        {
            if (!Projections.ContainsKey(dtoType)) return null;
            return Projections[dtoType].FirstOrDefault(t => t.Name == name);
        }
    }
}
