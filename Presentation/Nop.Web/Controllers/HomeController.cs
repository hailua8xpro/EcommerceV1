using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using ServiceStack;
using ServiceStack.Text;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}