using System.Reflection;

using BurgerFanatics.Domain.Domain.Models;
using BurgerFanatics.Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace BurgerFanatics.Api.Attachments;

public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// <para>This adds upload of attachments possible. This includes:</para>
    /// <para>POST /api/attachments for uploading a file</para>
    /// <para>GET /api/attachments/{id} for downloading a file</para>
    /// </summary>
    /// <param name="endpointRouteBuilder"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder AddAttachmentFeatures(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        Directory.CreateDirectory("files");
        endpointRouteBuilder.MapPost("/api/attachments", async (BurgerFanaticsDbContext context, UploadFileRequest request) =>
            {
                var allowedExtensions = new HashSet<string> { ".png", ".jpg", ".jpeg" };
                if (request.File is null)
                {
                    return Results.BadRequest();
                }

                if (!allowedExtensions.Contains(Path.GetExtension(request.File.FileName)))
                {
                    return Results.BadRequest(
                        $"Please provide a file with following extensions: {string.Join(',', allowedExtensions)}");
                }

                var id = Guid.NewGuid();
                var path = $"files/{id}";
                await using Stream fileStream = new FileStream(path, FileMode.Create);
                await request.File.CopyToAsync(fileStream);

                await context.FileAttachments.AddAsync(new FileAttachment
                {
                    FileAttachmentId = id,
                    FileName = request.File.FileName,
                    Path = path
                });

                await context.SaveChangesAsync();

                return Results.Ok(id);

            })
            .Accepts<UploadFileRequest>("multipart/form-data")
            .WithName("UploadAttachment");

        endpointRouteBuilder.MapGet("api/attachments/{id}",
            async (BurgerFanaticsDbContext context, Guid id) =>
                await context.FileAttachments.FirstOrDefaultAsync(x => x.FileAttachmentId == id) is { } file
                    ? Results.File(File.OpenRead(file.Path),
                        null,
                        file.FileName)
                    : Results.NotFound());
        return endpointRouteBuilder;
    }
}

// This is used because the minimal API (.NET 6) does not support formdata out of the box yet. 
// https://devblogs.microsoft.com/dotnet/asp-net-core-updates-in-net-7-preview-1/#iformfile-and-iformfilecollection-support
public class UploadFileRequest
{
    public IFormFile? File { get; set; }

    public static async ValueTask<UploadFileRequest?> BindAsync(HttpContext context, ParameterInfo parameterInfo)
    {
        var form = await context.Request.ReadFormAsync();
        var file = form.Files["file"];
        return new UploadFileRequest
        {
            File = file
        };
    }
}