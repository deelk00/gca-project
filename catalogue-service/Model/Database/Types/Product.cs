using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CatalogueService.Model.Database.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CatalogueService.Model.Database.Types;

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
    [Required]
    public Guid BrandId { get; set; }
    
    public string Name { get; set; }
    public uint Stock { get; set; }
    public Gender Gender { get; set; }
    public decimal Price { get; set; }

    public Currency? Currency { get; set; }
    public ProductCategory? ProductCategory { get; set; }
    public List<Tag>? Tags { get; set; }
    public List<FilterProperty>? FilterProperties { get; set; }
    public Brand? Brand { get; set; }
}