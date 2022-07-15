using System.ComponentModel.DataAnnotations;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;

namespace CatalogueService.Model.Database.Types;

[DynamicQL(options: QueryOptions.SingleCrud | QueryOptions.MultiQuery)]
public class Currency
{
    public Guid Id { get; set; }
    [MaxLength(128)]
    public string Name { get; set; }
    [MaxLength(4)]
    public string ShortName { get; set; }
    [MaxLength(4)]
    public string Symbol { get; set; }

    public List<Product>? Products { get; set; }
}