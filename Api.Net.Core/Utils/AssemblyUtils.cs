using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Api.Net.Core.Utils
{
    public static class AssemblyUtils
    {
        /// <summary>
        /// Returns all the assemblies and referenced assemblies of this current domain.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssemblies()
        {
            var result = new List<Assembly>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Adds the current assembly and its references
            var currentAssembly = Assembly.GetEntryAssembly();
            if (currentAssembly != null)
            {
                result.Add(currentAssembly);
                result.AddRange(GetReferencedAssemblies(currentAssembly));
            }
            
            foreach(var assembly in assemblies)
            {
                var referencedAssemblies = GetReferencedAssemblies(assembly);
                result.AddRange(referencedAssemblies);
            }

            // Ensure there is not duplicated assemblies
            return result.Distinct();
        }

        private static IEnumerable<Assembly> GetReferencedAssemblies(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies().Select(t => Assembly.Load(t)).ToList();
        }
    }
}
