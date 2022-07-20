using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
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

    public string Description { get; set; } = GenerateLoremIpsum(100,200);
    
    public Currency? Currency { get; set; }
    public ProductCategory? ProductCategory { get; set; }
    public List<Tag>? Tags { get; set; }
    public List<FilterProperty>? FilterProperties { get; set; }
    public List<ProductImage>? ProductImages { get; set; }
    public Brand? Brand { get; set; }


    [DynamicQLQueryResolver("products", true)]
    [DynamicQLResolverArgument("productCategoryId", typeof(GuidGraphType))]
    [DynamicQLResolverArgument("sortBy", typeof(EnumerationGraphType<SortBy>), DefaultValue = SortBy.Name)]
    [DynamicQLResolverArgument("sortByAscending", typeof(BooleanGraphType), DefaultValue = true)]
    [DynamicQLResolverArgument("maxPrice", typeof(FloatGraphType))]
    [DynamicQLResolverArgument("gender", typeof(EnumerationGraphType<Gender>))]
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
        var sortBy = context.GetArgument<SortBy>("sortBy");
        var sortByAscending = context.GetArgument<bool>("sortByAscending");
        var maxPrice = context.GetArgument<float?>("maxPrice");
        var gender = context.GetArgument<Gender?>("gender");

        var query = queryLoader.LoadDynamicQueries<Product>(dbContext, context);

        if (categoryId != Guid.Empty)
        {
            var category = await dbContext.Set<ProductCategory>()
                .Include(x => x.ChildCategories)
                .FirstOrDefaultAsync(x => x.Id == categoryId);

            if (category == null) throw new Exception("could not find category");

            var categoryIds = new List<Guid>();
            categoryIds.Add(category.Id);

            categoryIds.AddRange(category.ChildCategories!.Select(x => x.Id));

            query = query.Where(x => categoryIds.Contains(x.ProductCategoryId));
        }

        if (maxPrice != null)
        {
            var mp = (decimal)maxPrice;
            query = query.Where(x => (x.OfferPrice ?? x.Price) < mp);
        }

        if(gender != null) query = query.Where(x => gender == x.Gender);
        
        query = sortByAscending 
            ? query.OrderBy(GetSortFunction(sortBy)) 
            : query.OrderByDescending(GetSortFunction(sortBy));
        
        query = query.Skip(skip + pageSize * page)
            .Take((take + pageSize == 0) ? 50 : take + pageSize);

        return await query.ToListAsync();

    }

    private static Expression<Func<Product, dynamic>> GetSortFunction(SortBy sortBy)
    {
        switch (sortBy)
        {
            case SortBy.Price:
                return x => x.OfferPrice ?? x.Price;
            case SortBy.Name:
                return x => x.Name;
            default:
                throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, null);
        }
    }

    private static Random rnd = new Random();

    private static string[] words = new string[]
    {
        "Lorem", 
        "ipsum", 
        "dolor", 
        "sit", 
        "amet",
        ",", 
        "consetetur", 
        "sadipscing", 
        "elitr",
        ",", 
        "sed", 
        "diam", 
        "nonumy", 
        "eirmod", 
        "tempor", 
        "invidunt", 
        "ut", 
        "labore", 
        "et", 
        "dolore", 
        "magna", 
        "aliquyam", 
        "erat",
        "sed", 
        "diam", 
        "voluptua",
        ".",
    };
    private static string GenerateLoremIpsum(int minWordCount, int maxWordCount)
    {
        var text = new List<string>();
        var wordCounts = rnd.Next(minWordCount, maxWordCount);
        for (var i = 0; i < wordCounts; i++)
        {
            text.Add(words[rnd.Next(0, words.Length)]);
        }

        return string.Join(" ", text);
    }
}