using fileupload.MVC.Data;
using fileupload.MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace fileupload.MVC.Controllers;

public class FileController : Controller
{
    private readonly FileDbContext _context;
    private readonly IWebHostEnvironment _he;

    public FileController(FileDbContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _he = hostEnvironment;
    }


    public async Task<IActionResult> Index()
    {
        List<FileModel> files = await _context.Files.ToListAsync();
        return View(files);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFileAsync(List<IFormFile> files, string description)
    {
        foreach (var file in files)
        {
            var basePath = Path.Combine(_he.WebRootPath + "/files/");
            bool basePathExists = System.IO.Directory.Exists(basePath);
            if (!basePathExists) Directory.CreateDirectory(basePath);
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var filePath = Path.Combine(basePath, file.FileName);
            var extension = Path.GetExtension(file.FileName);
            if (!System.IO.File.Exists(filePath))
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileModel = new FileModel
                {
                    CreatedOn = DateTime.UtcNow,
                    FileName = fileName,
                    FileType = file.ContentType,
                    Extension = extension,
                    FileLocation = "files/" + file.FileName,
                    FileDescription = description
                };

                _context.Files.Add(fileModel);
                _context.SaveChanges();
            }
        }
        TempData["Message"] = "File successfully uploaded to File System.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> DownloadFileAsync(int id)
    {
        FileModel file = await _context.Files.FirstOrDefaultAsync(file => file.Id == id);
        var memory = new MemoryStream();
        using (var stream = new FileStream($"{_he.WebRootPath}/{file.FileLocation}", FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }

        memory.Position = 0;
        return File(memory, file.FileType, file.FileName + file.Extension);
    }

    public async Task<IActionResult> DeleteFileAsync(int id)
    {
        FileModel file = await _context.Files.FirstOrDefaultAsync(file => file.Id == id);
        if (System.IO.File.Exists($"{_he.WebRootPath}/{file.FileLocation}"))
            {
                System.IO.File.Delete($"{_he.WebRootPath}/{file.FileLocation}");
            }
            _context.Files.Remove(file);
            _context.SaveChanges();
            TempData["Message"] = $"Removed {file.FileName + file.Extension} successfully from File System.";
            return RedirectToAction("Index");
    }
}