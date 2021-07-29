using Api.Net.Dev.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Net.Dev.Models
{
    public partial class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }

        public virtual DbSet<Product> Products { get; set; }
    }
}
