using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserOwnsData.Models;
using UserOwnsData.Services;

namespace UserOwnsData.Controllers {

[Authorize]  
public class HomeController : Controller {

  private PowerBiServiceApi powerBiServiceApi;

  public HomeController(PowerBiServiceApi powerBiServiceApi) {
    this.powerBiServiceApi = powerBiServiceApi;
  }


    [AllowAnonymous]
    public IActionResult Index() {
      return View();
    }

public async Task<IActionResult> Embed() {

  Guid workspaceId = new Guid("912f2b34-7daa-4589-83df-35c75944d864");
  Guid reportId = new Guid("cd496c1c-8df0-48e7-8b92-e2932298743e");

  var viewModel = await powerBiServiceApi.GetReport(workspaceId, reportId);
  return View(viewModel);

}

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  
  }
}