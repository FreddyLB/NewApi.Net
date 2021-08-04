using System;
using Api.Parameters;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Api.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using Api;
using Api.Attributes;
using Api.Net.Core.Utils;
using Api.Net.Core.Metatada;
using Api.Models;

namespace Api.Controllers
{
    [ApiControllerModelConvention]
    public class ApiController<TDto> : Controller where TDto : class
    {
        public ApiController(IService<TDto> service, IListService listService)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
            ListService = listService ?? throw new ArgumentNullException(nameof(listService));
        }

        protected IService<TDto> Service { get; }
        protected IListService ListService { get; }

        [HttpGet]
        [Route("{id}")]
        public virtual ActionResult<TDto> Find(string id)
        {
            try
            {
                TDto dto = Service.Find(id);

                if (dto == null)
                    throw new ValidateException("The specified resource was not found");

                return Ok(dto);
            }
            catch (ValidateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.GetInnerMessages());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }

        [HttpGet]
        public virtual ActionResult<ListResult> GetAll(ApiParameter parameter)
        {
            try
            {
                var parameters = parameter.ProcessParameters(Request.Query);
                parameters.Filters.Add(DtoMetadata.Instance.Convention.ActiveProperty, true);
                var list = ListService.GetList(Service, parameters);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }

        [HttpPost]
        public virtual ActionResult<TDto> Add([FromBody] TDto dto)
        {
            try
            {
                var result = Service.Add(dto);
                // return CreatedAtAction(nameof(Find), result.GetValue<object>("Id"), result);
                return Ok(result);
            }
            catch (ValidateException ex)
            {
                Response.ContentType = "text/plain";
                return StatusCode((int)HttpStatusCode.BadRequest, ex.GetInnerMessages());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }

        [HttpPut]
        [Route("{id}")]
        public virtual ActionResult<TDto> Update(string id, [FromBody] TDto dto)
        {
            try
            {
                dto = Service.Update(id, dto);
                return Ok(dto);
            }
            catch (ValidateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.GetInnerMessages());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }


        [HttpPatch]
        [Route("{id}")]
        public virtual ActionResult<TDto> PartialUpdate(string id, [FromBody] object changes)
        {
            try
            {
                var dto = Service.Find(id);
                if (dto == null) throw new ValidateException("Resource not found");
                var patch = changes.ToJsonPatchDocument();
                patch.ApplyTo(dto);
                Service.Update(id, dto);
                return Ok(dto);
            }
            catch (ValidateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.GetInnerMessages());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual ActionResult<TDto> Delete(string id)
        {
            try
            {
                var result = Service.Delete(id);
                return Ok(result);
            }
            catch (ValidateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.GetInnerMessages());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }
    }
}
