using Fina.Api.Data;
using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Categories;
using Fina.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fina.Api.Handlers
{
    public class CategoryHandler(AppDbContext context) : ICategoryHandler
    {
        public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
        {
            var category = new Category
            {
                UserId = request.UserId,
                Title = request.Title,
                Description = request.Description
            };

            try
            {
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return new Response<Category?>(category, 201, "Categoria criada com sucesso");
            }
            catch
            {
                //Serilog, OpenTelem
                return new Response<Category?>(null, 500, "Nao foi possivel criar uma categoria");
            }
        }

        public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
        {
            try
            {
                var category = await context
                    .Categories
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

                if (category is null)
                    return new Response<Category?>(null, 404, "Categoria nao encontrada");

                category.Title = request.Title;
                category.Description = request.Description;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return new Response<Category?>(category, message: "Categoria atualizada com sucesso");
            }
            catch
            {
                //Serilog, OpenTelem
                return new Response<Category?>(null, 500, "Nao foi possivel atualizar categoria");
            }
        }

        public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
        {
            try
            {
                var category = await context
                    .Categories.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

                if (category is null)
                    return new Response<Category?>(null, 404, "Categoria nao encontrada");

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return new Response<Category?>(category, message: "Categoria excluida com sucesso");
            }
            catch
            {
                //Serilog, OpenTelem
                return new Response<Category?>(null, 500, "Nao foi possivel excluir categoria");
            }
        }

        public async Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
        {
            try
            {
                var category = await context.Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
                //.AsNoTrackingWithIdentityResolution() //Agrupar e criar indices

                return category is null
                    ? new Response<Category?>(null, 404, "Categoria nao encontrada")
                    : new Response<Category?>(category);
            }
            catch
            {
                //Serilog, OpenTelem
                return new Response<Category?>(null, 500, "Nao foi possivel encontrar categoria");
            }
        }


        public async Task<PagedResponse<List<Category>?>> GetAllAsync(GetAllCategoriesRequest request)
        {
            try
            {
                var query = context
                    .Categories
                    .AsNoTracking()
                    .Where(x => x.UserId == request.UserId)
                    .OrderBy(x => x.Title);

                var categories = await query
                    .Skip((request.PageNumber - 1) * request.PageSize) //Pular 
                    .Take(request.PageSize) //Pegar 
                    .ToListAsync();

                var count = await query .CountAsync();

                return new PagedResponse<List<Category>?>(
                    categories, count, request.PageNumber, request.PageSize);
            }
            catch
            {
                //Serilog, OpenTelem
                return new PagedResponse<List<Category>?>(null, 0, request.PageNumber, request.PageSize);
            }
        }
    }
}
