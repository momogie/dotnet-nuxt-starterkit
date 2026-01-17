using System.ComponentModel.DataAnnotations;

namespace Api.Handlers.Commands;

public class AttachmentUploadCommand
{
    public string DocumentId { get; set; }
    public string DocumentType { get; set; }
    [Required]
    public IFormFile File { get; set; }
}
