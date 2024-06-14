using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Categories;
using Fina.Core.Responses;
using System.Security.Claims;

namespace Fina.Api.Endpoints.Categories
{
    public class GetByIdCategoryEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
            => app.MapPost("/{id}", HandleAsync)
                .WithName("Categories: Get By Id")
                .WithSummary("Recuperar uma categoria")
                .WithDescription("Recuperar uma categoria")
                .WithOrder(4)
                .Produces<Response<Category?>>();

        private static async Task<IResult> HandleAsync(ClaimsPrincipal user, ICategoryHandler handler, long id)
        {
            var request = new GetCategoryByIdRequest
            {
                //UserId = user.Identity?.Name ?? string.Empty, //Pegar usuario logado
                UserId = ApiConfiguration.UserId, //configurado usuario
                Id = id
            };

            var responde = await handler.GetByIdAsync(request);

            return responde.IsSuccess
                ? TypedResults.Ok(responde)
                : TypedResults.BadRequest(responde);
        }
    }
}
