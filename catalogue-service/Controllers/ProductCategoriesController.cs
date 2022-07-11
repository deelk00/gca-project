﻿using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api;

namespace CatalogueService.Controllers;

[ApiController]
[Route("product-categories")]
public class ProductCategoryController : CrudController<ProductCategory>
{
    public ProductCategoryController(DbContext context) : base(context)
    {
    }
}