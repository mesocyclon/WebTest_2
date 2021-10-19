using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebTest_2.Models;

namespace WebTest_2.Controllers
{
    public class HomeController : Controller
    {

        private readonly SqlDBContext _dBContext;

        public HomeController()
        {
            _dBContext = new SqlDBContext();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EntityFrameworkCore()
        {
            ViewBag.User = _dBContext.Users.ToList();
            ViewBag.Phone = _dBContext.Phones.ToList();
            
            return View();
        }


     //   [Route("user-{Id}")]
        public IActionResult UserAdd()
        {

            User user = new User { Id = new Guid("30FB2ED3-EA0E-4F05-B0DB-EF6341A593F0"), SecondName = "gsdbjdz" ,FirstName = "William", LastName = "Shakespeare", Date1 = new DateTime(2010, 1, 18) };
            _dBContext.Add<User>(user);
            _dBContext.SaveChanges();

          //  User user = _dBContext.Users.Add(sc => sc.Id == id).FirstOrDefault();


         //   user.NumberViews++;
          //  _dBContext.SaveChanges();

            return View(user);
        }

        //  private readonly ILogger<HomeController> _logger;
        //
        //  public HomeController(ILogger<HomeController> logger)
        //  {
        //      _logger = logger;
        //  }
        //
        //  public IActionResult Index()
        //  {
        //      return View();
        //  }

        public IActionResult Privacy()
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
