using Api.Attributes;
using Api.Controllers;
using Api.Dto.Base;
using Api.Models;
using Api.Parameters;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Api.Net.Tests.Controllers
{
    public class ApiControllerTests : IDisposable
    {
        private static IServiceProvider Provider = GetServiceProvider();

        [Fact]
        public void FindTest()
        {
            var controller = GetController<PersonDto>();

            controller.Add(new PersonDto { Name = "Ana", Age = 23 });
            controller.Add(new PersonDto { Name = "Maria", Age = 30 });

            var v1 = controller.Find("1");
            var v2 = controller.Find("2");
            var v3 = controller.Find("3");

            Assert.IsType<OkObjectResult>(v1.Result);
            Assert.IsType<OkObjectResult>(v2.Result);
            Assert.IsType<NotFoundResult>(v3.Result);

            Assert.Equal(new PersonDto { Name = "Ana", Age = 23 }, v1.Value);

        }

        private static ApiController<T> GetController<T>() where T: class
        {
            var service = Provider.GetService<IService<T>>();
            var listService = Provider.GetService<IListService>();
            var controller = new ApiController<T>(service, listService);
            var httpContext = new DefaultHttpContext();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        private static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddControllers().AddApi(options =>
            {
                options.UseDbContext<PersonDbContext>(c =>
                {
                    return c.UseInMemoryDatabase(Guid.NewGuid().ToString());
                });
            });

            return services.BuildServiceProvider();
        }

        public void Dispose()
        {
            var dbContext = Provider.GetService<PersonDbContext>();
            dbContext.Clear();
        }
    }
}
