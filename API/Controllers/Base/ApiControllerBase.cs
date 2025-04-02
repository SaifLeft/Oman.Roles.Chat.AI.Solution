using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Base
{
    /// <summary>
    /// قاعدة التحكم الأساسية للواجهة البرمجية
    /// Base API controller with standard routing
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        // Base functionality can be added here
    }
}