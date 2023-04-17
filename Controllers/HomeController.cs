using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TestProject.Models;

namespace TestProject.Controllers
{
    public class HomeController : Controller
    {
        string mysqlCon = "Server=85.10.205.173;" +
            "Port=3306;" +
            "database=rubsarbrurueng;" +
            "user=a_d_m_i_n;" +
            "password=adminadmin;";
        public IActionResult Worker()
        {
            string? user = HttpContext.Session.GetString("username");
            if (user == null || string.IsNullOrEmpty(user)) {
                return RedirectToAction("login","User", new { area = "" });
            }

            return View();
        }
        public IActionResult Menu()
        {
            string? user = HttpContext.Session.GetString("username");
            if (user == null || string.IsNullOrEmpty(user))
            {
                return RedirectToAction("login", "User", new { area = "" });
            }
            ViewData.Clear();

            MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
            try
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCommand = new MySqlCommand("select * from `Restaurant`", mySqlConnection);
                MySqlDataReader reader = mySqlCommand.ExecuteReader();
                List<Restaurant> res = new List<Restaurant>();
                while (reader.Read())
                {
                    Restaurant tmp = new Restaurant();
                    tmp.Name = reader.GetString(1);
                    tmp.Description = reader.GetString(2);
                    tmp.img_source = reader.GetString(3);
                    res.Add(tmp);
                }
                ViewBag.res = res;
            }
            catch (Exception ex)
            {
                mySqlConnection.Close();
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View("~/Views/User/Error.cshtml");
            }
            finally
            {
                mySqlConnection.Close();
            }

            return View();
        }
        [HttpGet]
        public IActionResult Foodlist(string name)
        {
            string? user = HttpContext.Session.GetString("username");
            if (user == null || string.IsNullOrEmpty(user))
            {
                return RedirectToAction("login", "User", new { area = "" });
            }
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Menu", "Home", new { area = "" });
            }
            ViewBag.selectedRes = name;
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
            try
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCommand = new MySqlCommand("select * from `prathep` where Name='"+name+"';", mySqlConnection);
                MySqlDataReader reader = mySqlCommand.ExecuteReader();
                List<Food> foods = new List<Food>();
                while (reader.Read())
                {
                    Food food = new Food();
                    food.Name = reader.GetString(2);
                    food.Price = reader.GetString(3);
                    foods.Add(food);
                }
                ViewBag.foods = foods;
                if(foods.Count<=0)
                {
                    return RedirectToAction("Menu", "Home", new { area = "" });
                }
            }
            catch (Exception ex)
            {
                mySqlConnection.Close();
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View("~/Views/User/Error.cshtml");
            }
            finally
            {
                mySqlConnection.Close();
            }

            return View();
        }
    }
}
