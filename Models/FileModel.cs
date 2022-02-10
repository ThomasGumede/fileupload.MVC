namespace fileupload.MVC.Models;

public class FileModel
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string Extension { get; set; }
    public string FileLocation { get; set; }

    public string FileDescription { get; set; }
    public DateTime CreatedOn { get; set; }
}