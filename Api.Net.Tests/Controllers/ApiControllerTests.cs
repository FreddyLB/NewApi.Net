using Api.Attributes;
using Api.Controllers;
using Api.Dto.Base;
using Api.Parameters;
using Api.Services;
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

        public override bool Equals(object obj)
        {
            return obj is PersonDto dto &&
                   Id == dto.Id &&
                   Name == dto.Name &&
                   Age == dto.Age;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Age);
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PersonDto dto &&
                   Id == dto.Id &&
                   Name == dto.Name &&
                   Age == dto.Age;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Age);
        }
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
            var s = services.GetService<IService<PersonDto>>();
            var controller = new ApiController<PersonDto>();
            controller.Add(new PersonDto { Name = "Ana", Age = 23 });
            controller.Add(new PersonDto { Name = "Maria", Age = 30 });

            var result = controller.GetAll(new ApiParameter());
            Console.WriteLine(result);
            Assert.Empty(result.Value as IEnumerable<PersonDto>);            
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
