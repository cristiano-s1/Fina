using Fina.Api.Common.Api;
using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Categories;
using Fina.Core.Responses;
using System.Security.Claims;

namespace Fina.Api.Endpoints.Categories
{
    public class DeleteCategoryEndpoint: IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
            => app.MapPost("/{id}", HandleAsync)
                .WithName("Categories: Delete")
                .WithSummary("Excluir uma categoria")
                .WithDescription("Excluir uma categoria")
                .WithOrder(3)
                .Produces<Response<Category?>>();

        private static async Task<IResult> HandleAsync(ClaimsPrincipal user, ICategoryHandler handler, long id)
        {
            var request = new DeleteCategoryRequest
            {
                //UserId = user.Identity?.Name ?? string.Empty, //Pegar usuario logado
                UserId = ApiConfiguration.UserId, //configurado usuario
                Id = id
            };

            var responde = await handler.DeleteAsync(request);

            return responde.IsSuccess
                ? TypedResults.Ok(responde)
                : TypedResults.BadRequest(responde);
        }
    }
}
