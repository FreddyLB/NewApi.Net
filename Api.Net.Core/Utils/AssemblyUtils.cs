using System;
using System.Collections.Generic;
using System.IO;
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

            // Adds the current domain assemblies
            var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            result.AddRange(domainAssemblies);

            // Adds the current assembly and its references
            var currentAssembly = Assembly.GetEntryAssembly();
            if (currentAssembly != null) 
            { 
                result.Add(currentAssembly);
                result.AddRange(GetReferencedAssemblies(currentAssembly));
            }
            
            foreach(var assembly in domainAssemblies)
            {
                if (assembly != null)
                {
                    var referencedAssemblies = GetReferencedAssemblies(assembly);
                    result.AddRange(referencedAssemblies);
                }
            }

            // Ensure there is not duplicated assemblies
            return result.Distinct();
        }

        private static IEnumerable<Assembly> GetReferencedAssemblies(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies()
                .Select(LoadAssemblyOrNull)
                .Where(t => t != null)!;

            static Assembly? LoadAssemblyOrNull(AssemblyName assemblyName)
            {
                try
                {
                    return Assembly.Load(assemblyName);
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
            }
        }
    }
}
