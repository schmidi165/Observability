namespace Observability.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public partial class TestingController(
    ILogger<TestingController> logger    
): ControllerBase
{
    [LoggerMessage(0, LogLevel.Information, "This is testing logging for value={Value}")]
    partial void LogTesting(string value);

    [HttpGet]
    public IActionResult Get()
    {
        LogTesting("Hello from TestingController");
        return Ok("Testing is working!");
    }
}
