using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Services;
using Microsoft.EntityFrameworkCore.Storage;
using ToDoApp.Application.Dtos.SchoolModel;
using ToDoApp.Application.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using ToDoApp.Domains.Entities;
using ToDoApp.Domains.AppSettingsConfigurations;
using RazorLight;
using Microsoft.Extensions.Options;
using Hangfire;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[TypeFilter(typeof(AuthorizationFilter), Arguments = [new string[] {"Admin", "Staff"}])]

    //[Authorize(Roles = "Admin,Staff")]
    // hoặc truyền 1 chuỗi ròi băm ra
    // $"{nameof(Role.Admin)},{nameof(Role.Staff)}"

    //[Authorize]
    //[TypeFilter(typeof(TokenBlackListFilter))]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolService _schoolService;
        private readonly FileInformation _fileInformation;

        public SchoolController(
            ISchoolService schoolService,
            IOptions<FileInformation> fileInformationOptions)
        {
            _schoolService = schoolService;
            _fileInformation = fileInformationOptions.Value;
        }

        [HttpGet]
        public IEnumerable<SchoolViewModel> GetSchools(string? address)
        {
            return _schoolService.GetSchools(address);
        }

        [HttpGet("{id}/detail")]
        public SchoolStudentViewModel GetSchoolDetail(int id)
        {
            //var userId = HttpContext.Session.GetInt32("UserId");
            //var role = HttpContext.Session.GetString("Role");

            //if (userId == null || role != "Admin")
            //{
            //    return null;
            //}

            return _schoolService.GetSchoolDetail(id);
        }

        [HttpPost]
        public int PostSchool(SchoolCreateModel school)
        {
            return _schoolService.PostSchool(school);
        }

        [HttpPut]
        public int PutSchool(SchoolUpdateModel school)
        {
            return _schoolService.PutSchool(school);
        }

        [HttpDelete]
        public void DeleteSchool(int schoolId)
        {
            _schoolService.DeleteSchool(schoolId);
        }

        [HttpGet("excel")]
        public async Task<IActionResult> ExportSchools()
        {
            var schools = _schoolService.GetSchools(null);

            using var stream = new MemoryStream();
            using var excelFile = new ExcelPackage(stream);
            
            var workSheet = excelFile.Workbook.Worksheets.Add("school");
            workSheet.Cells[1, 1].LoadFromCollection(schools, true, TableStyles.Light1);

            await excelFile.SaveAsAsync(stream);

            return File(stream.ToArray(), "application/octet-stream", "schools.xlsx");
        }

        [HttpPost("excel")]
        public async Task<IActionResult> ImportSchools(IFormFile file)
        {
            var result = await _schoolService.ImportSchools(file);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> ExportSchoolPdf()
        {
            var htmlText = System.IO.File.ReadAllText("Template/SchoolReport.html");
            htmlText = htmlText.Replace("{{SchoolName}}", "Hehehe");

            var renderEngine = new ChromePdfRenderer();

            var pdf = await renderEngine.RenderHtmlAsPdfAsync(htmlText);

            var path = Path.Combine(_fileInformation.RootDirectory, "SchoolReport.pdf");
            pdf.SaveAs(path);

            return Ok();
        }

        [HttpGet("pdf-dynamic")]
        public async Task<IActionResult> ExportSchoolPdfDynamic()
        {
            var schools = _schoolService.GetSchools(null);

            var model = new SchoolReportModel
            {
                Author = "Duck",
                DateCreated = DateTime.Now.ToString("dd/MM/yyyy"),
                Schools = schools.ToList()
            };

            var engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(Path.Combine(Directory.GetCurrentDirectory(), "Template"))
                .UseMemoryCachingProvider()
                .Build();
 
            var htmlText = await engine.CompileRenderAsync("SchoolReportDynamic.cshtml", model);

            var renderEngine = new ChromePdfRenderer();

            var pdf = await renderEngine.RenderHtmlAsPdfAsync(htmlText);

            return File(pdf.BinaryData, "application/pdf", "SchoolReport.pdf");
        }

        [HttpGet("test-hangfire")]
        public async Task<IActionResult> TestHangfire()
        {
            // như setTimeout(func(), 0) nhưng chạy trong background;
            string jobId1 = BackgroundJob.Enqueue(() => Console.WriteLine("Hellooo"));

            string jobId2 = BackgroundJob.Schedule(() => Console.WriteLine("Hello, after 10 seconds"),
                TimeSpan.FromSeconds(10));

            string jobId3 = BackgroundJob.ContinueJobWith(jobId2,
                () => Console.WriteLine("This is a continuation job after jobId2"));

            BackgroundJob.ContinueJobWith(jobId3,
                () => Console.WriteLine("This is a continuation job after jobId3"));

            return Ok();
        }
    }
}
