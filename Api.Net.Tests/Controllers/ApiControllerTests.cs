using Api.Attributes;
using Api.Controllers;
using Api.Dto.Base;
using Api.Parameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Api.Net.Tests.Controllers
{
    [ApiEndpoint("persons")]
    public class PersonDto : Dto<PersonDto, Person>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class PersonDbContext : DbContext
    {
        public PersonDbContext(DbContextOptions<PersonDbContext> options) : base(options) { }

        public DbSet<Person> People { get; set; }
    }


    public class ApiControllerTests
    {
        [Fact]
        public void FindTest()
        {
            var services = GetServiceProvider();
            var controller = services.GetService<ApiController<PersonDto>>();

            var result = controller.GetAll(new ApiParameter());
            
        }

        private static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddControllers().AddApi(options =>
            {
                options.UseDbContext<PersonDbContext>(c =>
                {
                    return c.UseInMemoryDatabase("people");
                });
            });

            return services.BuildServiceProvider();
        }
    }
}
