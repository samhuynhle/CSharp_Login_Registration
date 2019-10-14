using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using login_reg.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace login_reg.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController (MyContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost("register")]
        public IActionResult Register(IndexViewModel modelData)
        {
            if(modelData == null)
            {
                return View("Index");
            }

            User submittedUser = modelData.newUser;
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == submittedUser.Email))
                {
                    ModelState.AddModelError("newUser.Email", "Email is already in use");
                    return View("Index");
                }

                dbContext.Users.Add(submittedUser);
                dbContext.SaveChanges();

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                submittedUser.Password = Hasher.HashPassword(submittedUser, submittedUser.Password);
                dbContext.SaveChanges();

                User current_user = dbContext.Users.FirstOrDefault(u => u.Email == submittedUser.Email);
                HttpContext.Session.SetInt32("Current_User_Id", current_user.UserId);
                int user_id = current_user.UserId;
                return Redirect($"dashboard/{user_id}");
            }
            return View("Index");
            
        }
        [HttpGet("login")]
        public IActionResult deadLogin()
        {
            return RedirectToAction("Index");
        }
        [HttpGet("register")]
        public IActionResult deadRegistration()
        {
            return RedirectToAction("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(IndexViewModel modelData)
        {
            if(modelData == null)
            {
                return View("Index");
            }
            LoginUser submittedUser = modelData.loginUser;
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u=> u.Email == submittedUser.Email);
                if (userInDb == null)
                {
                    ModelState.AddModelError("loginUser.Email", "Invalid Email/Password");
                    return View("Index");
                }
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(submittedUser, userInDb.Password, submittedUser.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("loginUser.Password", "Invalid Email/Password");
                    return View("Index");
                }
                User current_user = dbContext.Users.FirstOrDefault(u => u.Email == submittedUser.Email);
                HttpContext.Session.SetInt32("Current_User_Id", current_user.UserId);
                int user_id = current_user.UserId;
                return Redirect($"dashboard/{user_id}");
            }
            return View("Index");

        }
        [HttpGet("dashboard/{user_id}")]
        public IActionResult Success(int user_id)
        {
            int? current_user_id = HttpContext.Session.GetInt32("Current_User_Id");
            if(current_user_id == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }
            User current_user = dbContext.Users.FirstOrDefault(u => u.UserId == current_user_id);
            if(current_user.UserId != user_id)
            {
                return Redirect($"/dashboard/{current_user_id}");
            }

            return View();
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        // }
    }
}
