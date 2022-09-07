using FinancialAppDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FinancialAppDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ReadMe()
        {
            ViewBag.Title = "Readme";
            return View();
        }

        public IActionResult ToDoList()
        {
            return View();
        }

        public IActionResult GuessNum()
        {
            return View();
        }

        public IActionResult PigGame()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}