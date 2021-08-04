using Api.Attributes;
using Api.Dto.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Net.Tests
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
            return obj is Person dto &&
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
        
        public void Clear()
        {
            People.RemoveRange(People);
            SaveChanges();
        }
    }
}
