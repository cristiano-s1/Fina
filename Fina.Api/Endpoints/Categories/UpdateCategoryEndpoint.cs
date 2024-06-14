using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Categories;
using Fina.Core.Responses;
using System.Security.Claims;

namespace Fina.Api.Endpoints.Categories
{
    public class UpdateCategoryEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
            => app.MapPost("/{id}", HandleAsync)
                .WithName("Categories: Update")
                .WithSummary("Atualizar nova categoria")
                .WithDescription("Atualizar nova categoria")
                .WithOrder(2)
                .Produces<Response<Category?>>();

        private static async Task<IResult> HandleAsync(ICategoryHandler handler, UpdateCategoryRequest request, long id)
        {
            request.UserId = ApiConfiguration.UserId; 
            request.Id = id;

            var responde = await handler.UpdateAsync(request);

            return responde.IsSuccess
                ? TypedResults.Ok(responde)
                : TypedResults.BadRequest(responde);
        }
    }
}
