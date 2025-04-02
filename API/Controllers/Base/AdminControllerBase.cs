using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Base
{
    /// <summary>
    /// قاعدة التحكم الأساسية للمشرف
    /// Base admin controller with standard routing and admin authorization
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin/[controller]/[action]")]
    public abstract class AdminControllerBase : ControllerBase
    {
        // Base functionality for admin controllers can be added here
    }
}