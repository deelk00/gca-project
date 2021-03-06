using System.ComponentModel.DataAnnotations;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;

namespace CatalogueService.Model.Database.Types;

public class DatabaseImage
{
    public Guid Id { get; set; }
    [MaxLength(512)]
    public string FileName { get; set; }

    public string ContentType { get; set; }
    public byte[] Data { get; set; }
}