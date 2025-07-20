using Microsoft.Extensions.Options;
using RazorLight;
using ToDoApp.Application.Dtos.SchoolModel;
using ToDoApp.Application.Services;
using ToDoApp.Domains.AppSettingsConfigurations;

namespace ToDoApp.BackgroundJobs
{
    public class GenerateSchoolReportJob
    {
        private readonly ISchoolService _schoolService;
        private readonly FileInformation _fileInformation;

        public GenerateSchoolReportJob(
            ISchoolService schoolService,
            IOptions<FileInformation> fileInformation)
        {
            _schoolService = schoolService;
            _fileInformation = fileInformation.Value;
        }

        public async Task ExecuteAsync()
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

            var path = Path.Combine(_fileInformation.RootDirectory,
                $"SchoolReport-{DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss")}.pdf");
            pdf.SaveAs(path);
        }
    }
}
