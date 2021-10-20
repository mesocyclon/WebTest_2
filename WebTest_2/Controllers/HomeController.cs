﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebTest_2.Models;
using Newtonsoft.Json.Linq;

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

        public IActionResult Index()
        {
            ViewBag.Users       = _dBContext.Users.ToList();
            ViewBag.Phones      = _dBContext.Phones.ToList();
            ViewBag.PhoneList   = _dBContext.PhoneBooks.ToList();

            string phone_number = null;
            List<TableUserForm> UserList    = new List<TableUserForm>(); 

            foreach (User entity in ViewBag.Users)
            {
                List<TablePhones> PhonesUser = new List<TablePhones>();
                foreach (var L in ViewBag.PhoneList)
                {
                    if (L.UserId == entity.Id)
                    {
                        phone_number = getPhoneFromId(ViewBag.Phones, L.PhoneId) ;
                        PhonesUser.Add(new TablePhones { id = L.PhoneId, phone = phone_number });
                    }
                }                  

                UserList.Add(new TableUserForm {    id          = entity.Id , 
                                                    first_name  = entity.FirstName, 
                                                    last_name   = entity.LastName, 
                                                    second_name = entity.SecondName,
                                                    date_birth  = entity.DateBirth.ToString(),
                                                    phone_list  = PhonesUser.ToArray()
                });                
            }
            ViewBag.Users = UserList;

            string s = null;
            foreach(var entity in ViewBag.Users)
            {
                s = entity.id.ToString();
            }

            ViewBag.DataForm = "Связь пользователя и телефона";
            

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
        public IActionResult UsersPhoneEdit(Guid id)
        {
            Phone model = id == default ? new Phone() : GetPhoneById(id);
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
            System.Guid id_11 = new Guid("c6c4fbe0-8d64-4725-a520-08d993b2fabb");
            System.Guid id_12 = new Guid("6e60aa30-45e4-450f-04fd-08d993b8bf9d");
            ;            //PhoneBook model = id_1 == default ? new PhoneBook() : GetPBById(id_1,id_2);
            //return View(model);
            PhoneBook PhoneBook = new PhoneBook { PhoneId = id_12, UserId = id_11};
                   _dBContext.Add<PhoneBook>(PhoneBook);
                  _dBContext.SaveChanges();


            //  var insertCommand = "INSERT INTO Movies (Title, Genre, Year) VALUES(@0, @1, @2)";
            //  _dBContext.Execute(insertCommand, id_1, id_2);
              return View(PhoneBook);
        }

        //  public PhoneBook GetPBById(Guid id_1, Guid id_2)
        //  {
        //      return _dBContext.PhoneBooks.Single(x => x.UserId == id_2);
        //  }
        //
        //  [HttpPost]
        //  public IActionResult UsersToPhone(PhoneBook model)
        //  {
        //      if (ModelState.IsValid)
        //      {
        //          SavePhone(model);
        //          return RedirectToAction("Index");
        //      }
        //
        //      return View(model);
        //  }
        //
        //  public Guid SavePhone(PhoneBook entity)
        //  {
        //      if (entity.UserId == default)
        //          _dBContext.Entry(entity).State = EntityState.Added;
        //      else
        //          _dBContext.Entry(entity).State = EntityState.Modified;
        //      _dBContext.SaveChanges();
        //
        //      return entity.UserId;
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
