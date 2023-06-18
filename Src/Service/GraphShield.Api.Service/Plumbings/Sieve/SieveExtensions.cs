using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace GraphShield.Api.Service.Plumbings.Sieve
{
    internal static class SieveExtensions
    {
        /// <summary>
        /// Converts the given query to a paged response of the specified entity type using Sieve filtering and mapping.
        /// </summary>
        public static async Task<PagedResponse<TEntityDto>> ToPagedAsync<TEntity, TEntityDto>(
            this IQueryable<TEntity> query, ISieveProcessor sieve, IMapper mapper, SieveModel? model,
            CancellationToken cancellationToken = default) where TEntityDto : class
        {
            var page = model?.Page ?? 1;
            var pageSize = model?.PageSize ?? 1;

            if (model is not null)
                query = sieve.Apply(model, query, applyPagination: false);

            var rowCount = await query.CountAsync(cancellationToken);
            var pageCount = (int)Math.Ceiling((double)rowCount / pageSize);

            var skip = (page - 1) * pageSize;
            var pagedQuery = query.Skip(skip).Take(pageSize);

            return new PagedResponse<TEntityDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                PageCount = pageCount,
                RawCount = rowCount,
                Results = await pagedQuery.ProjectTo<TEntityDto>(mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken)
            };
        }
    }
}