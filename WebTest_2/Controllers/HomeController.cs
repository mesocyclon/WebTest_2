using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebTest_2.Models;
using Newtonsoft.Json.Linq;
using ClosedXML.Excel;
using System.IO;

namespace WebTest_2.Controllers
{
    public class HomeController : Controller
    {

        private readonly SqlDBContext _dBContext;

        public class TableUserForm
        {
            public Guid id { get; set; }
            public string last_name { get; set; }
            public string first_name { get; set; }
            public string second_name { get; set; }
            public string date_birth { get; set; }
            public Array phone_list { get; set; }
        }

        public class TablePhones
        {
            public Guid id { get; set; }
            public string phone { get; set; }
        }


        public HomeController()
        {
            _dBContext = new SqlDBContext();
        }

        public ActionResult Export()
        {
            List<User> UserE = _dBContext.Users.ToList();
            List<Phone> PhoneE = _dBContext.Phones.ToList();


            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                //Examples: https://github.com/ClosedXML/ClosedXML/tree/develop/ClosedXML.Examples

                var worksheetUsers = workbook.Worksheets.Add("Пользователи");


                int row = 1;
                int col = 2;

                worksheetUsers.Range(worksheetUsers.Cell(row, col), worksheetUsers.Cell(row, col+4)).Merge();


                var columnFromRange = worksheetUsers.Range("B2:G2").FirstRow();

                columnFromRange.Cell(1).Style.Fill.BackgroundColor = XLColor.Red;
                columnFromRange.Cells("2").Style.Fill.BackgroundColor = XLColor.Blue;
                columnFromRange.Cells("3,5:6").Style.Fill.BackgroundColor = XLColor.Red;
              //  columnFromRange.Cells(8, 9).Style.Fill.BackgroundColor = XLColor.Blue;

                worksheetUsers.Cell("B2").Value = "Фамилия";
                worksheetUsers.Cell("C2").Value = "Имя";
                worksheetUsers.Cell("D2").Value = "Отчество";
                worksheetUsers.Cell("E2").Value = "Дата рождения";
                worksheetUsers.Row(2).Style.Font.Bold = true;

                //нумерация строк/столбцов начинается с индекса 1 (не 0)
                for (int i = 0; i < UserE.Count; i++)
                {
                    worksheetUsers.Cell(i + 3, 2).Value = UserE[i].FirstName;
                    worksheetUsers.Cell(i + 3, 3).Value = UserE[i].LastName;
                    worksheetUsers.Cell(i + 3, 4).Value = UserE[i].SecondName;
                    worksheetUsers.Cell(i + 3, 5).Value = UserE[i].DateBirth;
                    worksheetUsers.Cell(i + 3, 5).DataType = XLDataType.DateTime;
                    
                }

                var worksheetPhone = workbook.Worksheets.Add("Телефоны");


                worksheetPhone.Cell("A1").Value = "ID";
                worksheetPhone.Cell("B1").Value = "Телефон";
               
                worksheetPhone.Row(1).Style.Font.Bold = true;

                //нумерация строк/столбцов начинается с индекса 1 (не 0)
                for (int i = 0; i < PhoneE.Count; i++)
                {
                    worksheetPhone.Cell(i + 2, 1).Value = PhoneE[i].Id;
                    worksheetPhone.Cell(i + 2, 2).Value = PhoneE[i].PhoneNumber;
                   
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"exports_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }

        public IActionResult Index()
        {
            ViewBag.Users = _dBContext.Users.ToList();
            ViewBag.Phones = _dBContext.Phones.ToList();
            ViewBag.PhoneList = _dBContext.PhoneBooks.ToList();

            string phone_number = null;
            List<TableUserForm> UserList = new List<TableUserForm>();

            foreach (User entity in ViewBag.Users)
            {
                List<TablePhones> PhonesUser = new List<TablePhones>();
                foreach (var L in ViewBag.PhoneList)
                {
                    if (L.UserId == entity.Id)
                    {
                        phone_number = getPhoneFromId(ViewBag.Phones, L.PhoneId);
                        PhonesUser.Add(new TablePhones { id = L.PhoneId, phone = phone_number });
                    }
                }

                UserList.Add(new TableUserForm
                {
                    id = entity.Id,
                    first_name  = entity.FirstName,
                    last_name   = entity.LastName,
                    second_name = entity.SecondName,
                    date_birth = entity.DateBirth.ToString(),
                    phone_list = PhonesUser.ToArray()
                });
            }
            ViewBag.Users = UserList;

            string s = null;
            foreach (var entity in ViewBag.Users)
            {
                s = entity.id.ToString();
            }

            ViewBag.DataForm = "Привет Тест";

            return View();
        }

        private string getPhoneFromId(List<Phone> aPhones, Guid aId)
        {
            string s = null;
            foreach (Phone p in aPhones)
            {
                if (p.Id == aId)
                {
                    s = p.PhoneNumber;
                    break;
                }
            }
            return s;
        }


        public IActionResult EntityFrameworkCore()
        {
            ViewBag.Users = _dBContext.Users.ToList();
            ViewBag.Phones = _dBContext.Phones.ToList();

            return View();
        }


        public User GetUserById(Guid id)
        {
            return _dBContext.Users.Single(x => x.Id == id);
        }

        public Guid SaveUser(User entity)
        {
            if (entity.Id == default)
                _dBContext.Entry(entity).State = EntityState.Added;
            else
                _dBContext.Entry(entity).State = EntityState.Modified;
            _dBContext.SaveChanges();

            return entity.Id;
        }

        public IActionResult UsersEdit(Guid id)
        {
            User model = id == default ? new User() : GetUserById(id);
            return View(model);
        }
        [HttpPost]
        public IActionResult UsersEdit(User model)
        {
            if (ModelState.IsValid)
            {
                SaveUser(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public void DeleteUser(User entity)
        {
            _dBContext.Users.Remove(entity);
            _dBContext.SaveChanges();
        }

        [HttpPost]
        public IActionResult UsersDelete(Guid id)
        {
            DeleteUser(new User { Id = id });
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>


        public Phone GetPhoneById(Guid id)
        {
            return _dBContext.Phones.Single(x => x.Id == id);
        }

        public Guid SavePhone(Phone entity)
        {
            if (entity.Id == default)
                _dBContext.Entry(entity).State = EntityState.Added;
            else
                _dBContext.Entry(entity).State = EntityState.Modified;
            _dBContext.SaveChanges();

            return entity.Id;
        }

        public IActionResult PhoneEdit(Guid id)
        {
            Phone model = id == default ? new Phone() : GetPhoneById(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult UsersPhoneEdit()
        { 
            Guid idPhone = new Guid();
            foreach (var row in Request.Form)
            {
                if (row.Key == "phone-id")
                    idPhone = new Guid(row.Value);
            }
           
            Phone PhoneNum = idPhone == default ? new Phone() : GetPhoneById(idPhone);
           
            ViewBag.PhoneNum = PhoneNum;
            return View("~/Views/Home/Phones/PhoneEdit.cshtml");
        }

        [HttpPost]
        public IActionResult PhoneEdit(Phone model)
        {
            if (ModelState.IsValid)
            {
                SavePhone(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public void DeletePhone(Phone entity)
        {
            _dBContext.Phones.Remove(entity);
            _dBContext.SaveChanges();
        }



        [HttpPost]
        public IActionResult PhoneDelete(Guid id)
        {
            DeletePhone(new Phone { Id = id });
            return RedirectToAction("Index");
        }



        //   [Route("user-{Id}")]
        //   public IActionResult UserAdd()
        //   {
        //
        //       User user = new User {  SecondName = "gsdbjdz" ,FirstName = "William", LastName = "Shakespeare", Date1 = new DateTime(2010, 1, 18) };
        //       _dBContext.Add<User>(user);
        //       _dBContext.SaveChanges();
        //
        //     //  User user = _dBContext.Users.Add(sc => sc.Id == id).FirstOrDefault();
        //
        //
        //    //   user.NumberViews++;
        //     //  _dBContext.SaveChanges();
        //
        //       return View(user);
        //   }

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

        public IActionResult UsersToPhone(Guid id_1, Guid id_2)
        {
            PhoneBook model = id_1 == default ? new PhoneBook() : GetPBById(id_1, id_2);
            return View(model);
        }

        public PhoneBook GetPBById(Guid id_1, Guid id_2)
        {
            return _dBContext.PhoneBooks.Single(x => x.UserId == id_2);
        }

        [HttpPost]
        public IActionResult UsersToPhone(PhoneBook model)
        {
            if (ModelState.IsValid)
            {
                SavePhone(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public Guid SavePhone(PhoneBook entity)
        {
            if (entity.UserId == default)
                _dBContext.Entry(entity).State = EntityState.Added;
            else
                _dBContext.Entry(entity).State = EntityState.Modified;
            _dBContext.SaveChanges();

            return entity.UserId;
        }

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
