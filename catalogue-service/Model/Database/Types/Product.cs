using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CatalogueService.Model.Database.Enums;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;
using DynamicQL.Services;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogueService.Model.Database.Types;

[DynamicQL("product", "products", 50, QueryOptions.SingleQuery)]
public class Product
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(ProductCategory))]
    [Required]
    public Guid ProductCategoryId { get; set; }
    
    [ForeignKey(nameof(Currency))] 
    [Required]
    public Guid CurrencyId { get; set; }
    
    [ForeignKey(nameof(Brand))]
    public Guid? BrandId { get; set; }
    
    public string Name { get; set; }
    public uint Stock { get; set; }
    public Gender Gender { get; set; }
    public decimal Price { get; set; }
    public decimal? OfferPrice { get; set; }

    public Currency? Currency { get; set; }
    public ProductCategory? ProductCategory { get; set; }
    public List<Tag>? Tags { get; set; }
    public List<FilterProperty>? FilterProperties { get; set; }
    public List<ProductImage>? ProductImages { get; set; }
    public Brand? Brand { get; set; }


    [DynamicQLQueryResolver("products", true)]
    [DynamicQLResolverArgument("productCategoryId", typeof(GuidGraphType))]
    [DynamicQLResolverArgument("skip", typeof(IntGraphType))]
    [DynamicQLResolverArgument("take", typeof(IntGraphType))]
    [DynamicQLResolverArgument("page", typeof(IntGraphType))]
    [DynamicQLResolverArgument("pageSize", typeof(IntGraphType))]
    public async Task<object> GetProductList(IResolveFieldContext context, DbContext dbContext, IDynamicQueryLoaderService queryLoader)
    {
        var categoryId = context.GetArgument<Guid>("productCategoryId");
        var skip = context.GetArgument<int>("skip");
        var take = context.GetArgument<int>("take");
        var page = context.GetArgument<int>("page");
        var pageSize = context.GetArgument<int>("pageSize");

        if (categoryId == Guid.Empty)
            return await queryLoader.LoadDynamicQueries<Product>(dbContext, context)
                .Skip(skip + pageSize * page)
                .Take((take + pageSize == 0) ? 50 : take + pageSize)
                .ToListAsync();

        var category = await dbContext.Set<ProductCategory>()
            .Include(x => x.ChildCategories)
            .FirstOrDefaultAsync(x => x.Id == categoryId);

        if (category == null) throw new Exception("could not find category");

        var categoryIds = new List<Guid>();
        categoryIds.Add(category.Id);

        categoryIds.AddRange(category.ChildCategories!.Select(x => x.Id));

        return await queryLoader.LoadDynamicQueries<Product>(dbContext, context)
            .Where(x => categoryIds.Contains(x.ProductCategoryId))
            .Skip(skip + pageSize * page)
            .Take((take + pageSize == 0) ? 50 : take + pageSize)
            .ToListAsync();

    }
}