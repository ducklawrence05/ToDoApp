using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ToDoApp.Domains.AppSettingsConfigurations;

namespace ToDoApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly FileInformation _fileInformation;

        public FileController(IOptions<FileInformation> fileInformation)
        {
            _fileInformation = fileInformation.Value;
        }

        [HttpGet("{fileName}/read")]
        public async Task<ActionResult> ReadFileAsync(string fileName)
        {
            //var content = await System.IO.File.ReadAllLinesAsync(path);
            //return Ok(content);

            // Dùng Stream thì thêm using để sau khi hết hàm thì tự xoá khỏi heap luôn
            //  ko cần chờ garbage collector tới chu trình quét và xoá
            //  giúp tăng performance (ko để memory tràn với những file quá lớn)

            var path = Path.Combine(_fileInformation.RootDirectory, fileName);

            if (!System.IO.File.Exists(path))
            {
                return NotFound($"File '{fileName}' not found.");
            }

            using var reader = new StreamReader(path);

            string? line = null;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
            return Ok();
        }

        [HttpPost("{fileName}/write")]
        public async Task<ActionResult> WriteFileAsync(string fileName, string content)
        {
            //await System.IO.File.WriteAllTextAsync(path, content);
            //await System.IO.File.AppendAllTextAsync(path, content);

            var path = Path.Combine(_fileInformation.RootDirectory, fileName);
            if (!System.IO.File.Exists(path))
            {
                // nếu fileName là Test\\fileName mà folder Test thì ko có
                //  thì sẽ tự tạo folder theo đường dẫn cái truyền vào
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            
            // nếu file ko có thì tự tạo file và ghi vào
            using var writer = new StreamWriter(path, append: true);

            await writer.WriteLineAsync(content);

            return Ok();
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded of file is empty.");
            }

            var path = Path.Combine(_fileInformation.RootDirectory, file.FileName);

            using var stream = new FileStream(path, FileMode.Create);
            try
            {
                await file.CopyToAsync(stream);
                return Ok($"File '{file.FileName}' uploaded successfully");
            } 
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{fileName}/download")]
        public async Task<ActionResult> DownloadFileAsync(String fileName)
        {
            var path = Path.Combine(_fileInformation.RootDirectory, fileName);

            if (!System.IO.File.Exists(path))
            {
                return NotFound($"File '{fileName}' does not exist");
            }
            
            try
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(path);

                // cái dưới là để khai báo là loại file nào, còn octet-stream là như thằng cha
                //  dùng khi ko biết là file nào
                //  và dotnet cũng có hàm built-in sẵn để biết là loại file nào
                var contentType = "application/octet-stream";

                // attachment: force download of the file
                Response.Headers["Content-Disposition"] = $"attachment; filename = \"{fileName}\"";

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{fileName}")]
        public async Task<ActionResult> OpenFileInBrowserAsync(String fileName)
        {
            var path = Path.Combine(_fileInformation.RootDirectory, fileName);

            if (!System.IO.File.Exists(path))
            {
                return NotFound($"File '{fileName}' does not exist");
            }

            try
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(path);

                // cái dưới là để khai báo là loại file nào, còn octet-stream là như thằng cha
                //  dùng khi ko biết là file nào
                //  và dotnet cũng có hàm built-in sẵn để biết là loại file nào
                var contentType = "application/pdf";

                // inline: display file inside the browser (e.g., PDF, images)
                Response.Headers["Content-Disposition"] = $"inline; filename = \"{fileName}\"";
                // trong return phải xoá fileName nếu dùng inline, nếu có fileName là coi như thành attachment
                return File(fileBytes, contentType);

                //return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
