using System.Collections;
using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Other.Extensions;
using System;
using Utility.EFCore;
using Utility.Other.Enums;

namespace CatalogueService.Controllers;

[ApiController]
[Route("images")]
public class ImagesController : Controller
{
    private const int PageSize = 50;
    private readonly DbContext _context;
    
    public ImagesController(DbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("download/{id:guid}")]
    public async Task<ActionResult> DownloadImage([FromRoute] Guid id)
    {
        var dbImage = await _context.Set<Image>()
            .Include(x => x.DatabaseImage)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (dbImage?.DatabaseImage == null) return BadRequest("image not found");
        return File(dbImage.DatabaseImage.Data, dbImage.DatabaseImage.ContentType, dbImage.DatabaseImage.FileName);
    }
    
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<Image>> Get([FromRoute] Guid id)
    {
        return Ok((await _context.FindAsync<Image>(id))?.ToResponseDict());
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Image>>> GetList([FromQuery] int page = 0)
    {
        return Ok((await _context.Set<Image>()
            .Skip(PageSize * page)
            .Take(PageSize)
            .ToListAsync()
            ).Select(x => x.ToResponseDict()));
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<Image>> Post([FromForm] string? md5Checksum, 
        [FromForm] string? fileName = null
        )
    {
        if (Request.Form.Files.Count != 1) return BadRequest("one image expected");
        var file = Request.Form.Files[0];
        if (file.Length > 512 * 1024) return BadRequest("image is too large");
        var contentType = file.Headers.ContentType;

        if (string.IsNullOrEmpty(fileName))
        {
            fileName = file.Name;
        }
        if (fileName.Length > 512) return BadRequest("the filename is to large");
        
        if (contentType != "image/gif"
            && contentType != "image/png"
            && contentType != "image/jpeg"
            && contentType != "image/tiff"
            ) return BadRequest("only images are allowed");
        
        var content = new byte[file.Length];
        var stream = file.OpenReadStream();

        var bytesRead = await stream.ReadAsync(content.AsMemory(0, (int)file.Length));
        if (bytesRead != file.Length) return BadRequest("upload failed");

        var md5 = content.GetChecksum(HashAlgorithm.Md5);
        if (md5Checksum != null && md5Checksum != md5) return BadRequest("the provided hash does not equal the calculated md5 checksum");
        
        var otherImg = await _context.Set<Image>().FirstOrDefaultAsync(x => x.Hash == md5);
        Image? image = null;
        DatabaseImage? dbImage = null;
        
        if (otherImg != null)
        {
            dbImage = await _context.FindAsync<DatabaseImage>(otherImg.DatabaseImageId);
            if (dbImage != null)
            {
                if (content.Length == dbImage.Data.Length
                    && content.SequenceEqual(dbImage.Data))
                {
                    image = new Image()
                    {
                        Hash = md5,
                        DatabaseImageId = otherImg.DatabaseImageId
                    };
                }
                else
                {
                    dbImage = null;
                }
            }
            else
            {
                dbImage = null;
            }
        }

        if (image != null && dbImage != null)
        {
            image = await _context.TransactionAsync(x => _context.AddAsync(image));
            return Ok(image.ToResponseDict());
        }
        
        dbImage = new DatabaseImage()
        {
            Data = content,
            FileName = fileName,
            ContentType = contentType
        };
        image = new Image()
        {
            Hash = md5,
            DatabaseImage = dbImage
        };

        image = await _context.TransactionAsync(async transaction => await _context.AddAsync(image));

        return Ok(image.ToResponseDict());
    }

    [HttpPut]
    [DisableRequestSizeLimit]
    [Route("{id:guid}")]
    public async Task<ActionResult<Image>> Put([FromRoute] Guid id, [FromForm] string md5Checksum)
    {
        if (Request.Form.Files.Count != 1) return BadRequest("one image expected");
        var file = Request.Form.Files[0];
        if (file.Length > 512 * 1024) return BadRequest("image is too large");
        
        var content = new byte[file.Length];
        var stream = file.OpenReadStream();

        var bytesRead = await stream.ReadAsync(content.AsMemory(0, (int)file.Length));
        if (bytesRead != file.Length) return BadRequest("upload failed");

        if (content.IsImage(ImageType.All)) return BadRequest("only images are allowed");

        var md5 = content.GetChecksum(HashAlgorithm.Md5);
        if (md5Checksum != md5) return BadRequest("the provided hash does not equal the calculated md5 checksum");
        
        var image = await _context.FindAsync<Image>(id);
        if (image == null) return BadRequest("image not found");
        
        var dbImage = await _context.FindAsync<DatabaseImage>(image.DatabaseImageId);;
        if (dbImage == null) throw new Exception("DatabaseImage could not be found");
        
        dbImage.Data = content;
        image.Hash = md5;

        image = await _context.TransactionAsync(x => _context.Update(image));
        dbImage = await _context.TransactionAsync(x => _context.Update(dbImage));

        return Ok(image.ToResponseDict());
    }
    
    
    [HttpDelete]
    [DisableRequestSizeLimit]
    [Route("{id:guid}")]
    public async Task<ActionResult<Image>> Delete([FromRoute] Guid id)
    {
        var image = await _context.FindAsync<Image>(id);
        if (image == null) return BadRequest("no image found");

        image = await _context.TransactionAsync(x => _context.Remove(image));

        var connectedDbImageCount = await _context.Set<Image>()
            .CountAsync(x => x.DatabaseImageId == image.DatabaseImageId);

        var dbImage = await _context.FindAsync<DatabaseImage>(image.DatabaseImageId);

        if (connectedDbImageCount == 0 && dbImage != null) 
            await _context.TransactionAsync(x => _context.Remove(dbImage));

        return Ok(image.ToResponseDict());
    }
}