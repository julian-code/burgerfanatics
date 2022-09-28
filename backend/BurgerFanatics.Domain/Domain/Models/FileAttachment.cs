namespace BurgerFanatics.Domain.Domain.Models;

public class FileAttachment
{
    public Guid FileAttachmentId { get; set; }
    public string Path { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public Guid? ReviewId { get; set; }

    public virtual Review? Review { get; set; }
}