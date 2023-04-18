using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Collections.Concurrent;
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
        private static ConcurrentDictionary<string, Pair<string, int>> fooddata = new ConcurrentDictionary<string, Pair<string, int>>();
        private static ConcurrentDictionary<int, ConcurrentDictionary<string, Pair<string, int>>> shoppingcartdata = new ConcurrentDictionary<int, ConcurrentDictionary<string, Pair<string, int>>>(); // list of order
        private static int idx = 0; //for indexing shoppingcartdata
        public IActionResult Worker()
        {
            string? user = HttpContext.Session.GetString("username");
            if (user == null || string.IsNullOrEmpty(user))
            {
                return RedirectToAction("login", "User", new { area = "" });
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

            fooddata = new ConcurrentDictionary<string, Pair<string, int>>();
            try
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCommand = new MySqlCommand("select * from `prathep` where Name='" + name + "';", mySqlConnection);
                MySqlDataReader reader = mySqlCommand.ExecuteReader();
                List<Food> foods = new List<Food>();
                //var foodcount = new Dictionary<string,int>();
                while (reader.Read())
                {
                    Food food = new Food();
                    food.Name = reader.GetString(2);
                    food.Price = reader.GetString(3);

                    //foodcount[food.Name] = 0;  // for increse and decrese value of order

                    foods.Add(food);
                }
                ViewBag.foods = foods;
                //TempData["foodcount"] = foodcount;
                if (foods.Count <= 0)
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
        [HttpPost]
        public IActionResult Foodlist() //<--- submit to cart
        {
            string? user = HttpContext.Session.GetString("username");
            if (user == null || string.IsNullOrEmpty(user))
            {
                return RedirectToAction("login", "User", new { area = "" });
            }
            shoppingcartdata.TryAdd(idx, fooddata);
            idx++;
            fooddata = new ConcurrentDictionary<string, Pair<string, int>>();
            return RedirectToAction("ShoppingCart", "Home", new { area = "" });
        }

        [HttpPost]
        public IActionResult IncreaseValueBy(string foodname, string foodprice, int value)
        {
            if (!fooddata.ContainsKey(foodname))
            {
                fooddata.TryAdd(foodname, new Pair<string, int>(foodprice, 0)); //price, count
            }
            Pair<string, int> tmp;
            fooddata.TryGetValue(foodname, out tmp);
            tmp.Second += value;
            if (tmp.Second < 0)
            {
                tmp.Second = 0;
                fooddata.TryRemove(foodname, out _);
                return Json(0);
            }
            else
            {
                fooddata.TryUpdate(foodname, tmp, fooddata[foodname]);
                return Json(tmp.Second);
            }
        }

        public IActionResult ShoppingCart()
        {
            string? user = HttpContext.Session.GetString("username");
            if (user == null || string.IsNullOrEmpty(user))
            {
                return RedirectToAction("login", "User", new { area = "" });
            }

            ViewBag.shoppingcartdata = shoppingcartdata;
            return View();
        }
        [HttpPost]
        public IActionResult ShoppingCartIncreaseValueBy(int orderkey, string foodname, int value)
        {
            ConcurrentDictionary<string, Pair<string, int>> tmp;
            Pair<string, int> tmpPair;

            shoppingcartdata.TryGetValue(orderkey, out tmp);   // get one list of order to tmp
            tmp.TryGetValue(foodname, out tmpPair);            // get Pair<cost,count> from tmp to tmpPair

            tmpPair.Second += value;
            if (tmpPair.Second < 0) { tmpPair.Second = 0; }

            tmp.TryUpdate(foodname, tmpPair, tmp[foodname]);  // put back to tmp
            shoppingcartdata.TryUpdate(orderkey, tmp, shoppingcartdata[orderkey]); // put back to shoppingcartdata

            return Json(tmpPair.Second);
        }
        [HttpPost]
        public IActionResult deleteOrder(int orderkey)
        {
            shoppingcartdata.TryRemove(orderkey, out _);
            return Json("success");
        }
    }
}
