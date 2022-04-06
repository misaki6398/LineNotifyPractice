using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LineNotifyPractice.Models;
using LineNotifyPractice.Models.DB;

namespace LineNotifyPractice.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index([FromServices] SubscriberContext db)
    {

        var userId = HttpContext.Session.GetString("userId");
        if (HttpContext.Session.GetString("userId") is not null)
        {
            var subscriber = db.Subscribers.FirstOrDefault(c => c.LINEUserId == userId);
            return View(subscriber);
        };       
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
