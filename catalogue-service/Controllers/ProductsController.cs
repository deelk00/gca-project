﻿using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api;
using Utility.Api.Controllers;

namespace CatalogueService.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : CrudController<Product>
{
    public ProductsController(DbContext context) : base(context)
    {
    }
}