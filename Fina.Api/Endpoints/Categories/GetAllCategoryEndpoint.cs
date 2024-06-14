using Fina.Api.Common.Api;
using Fina.Core;
using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Categories;
using Fina.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fina.Api.Endpoints.Categories
{
    public class GetAllCategoryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
            => app.MapPost("/", HandleAsync)
                .WithName("Categories: Get All")
                .WithSummary("Recuperar todas as categoria")
                .WithDescription("Recuperar todas as categoria")
                .WithOrder(5)
                .Produces<PagedResponse<List<Category>?>>();

        private static async Task<IResult> HandleAsync(ICategoryHandler handler,
            [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
            [FromQuery] int pageSize = Configuration.DefaultPageSize)
        {
            var request = new GetAllCategoriesRequest
            {
                //UserId = user.Identity?.Name ?? string.Empty, //Pegar usuario logado
                UserId = ApiConfiguration.UserId, //configurado usuario
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var responde = await handler.GetAllAsync(request);

            return responde.IsSuccess
                ? TypedResults.Ok(responde)
                : TypedResults.BadRequest(responde);
        }
    }
}
