using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Collections.Concurrent;
using System.Data.SqlClient;
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
        //private static ConcurrentDictionary<string, Pair<string, int>> fooddata = new ConcurrentDictionary<string, Pair<string, int>>();
        //private static ConcurrentDictionary<int, ConcurrentDictionary<string, Pair<string, int>>> shoppingcartdata = new ConcurrentDictionary<int, ConcurrentDictionary<string, Pair<string, int>>>(); // list of order
        //private static int idx = 0; //for indexing shoppingcartdata
        private static ConcurrentDictionary<int, Food> shoppingcartdata = new ConcurrentDictionary<int, Food>(); //list of order
        private static int idx = 0; //for indexing shoppingcartdata
        private const int orderlimit = 4;
        private static int acceptedOrderID = -1; //for track customer and worker id in database
        public static void clearShoppingCart()
        {
            idx = 0;
			acceptedOrderID = -1;
			shoppingcartdata.Clear();
        }
        //============================================= Worker ====================================================
        public IActionResult Worker()
        {
            string? user = HttpContext.Session.GetString("username");
            if (user == null || string.IsNullOrEmpty(user))
            {
                return RedirectToAction("login", "User", new { area = "" });
            }

            return View();
        }
        public JsonResult GetNotifications()
        {
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
            try
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCommand = new MySqlCommand("select * from bill where status='waiting for rider' order by id asc limit 1;", mySqlConnection);
                MySqlDataReader reader = mySqlCommand.ExecuteReader();

                if (reader.Read())
                {
                    string id = reader.GetString(0);
                    mySqlConnection.Close();
                    return Json(id);
                }
                else
                {
                    mySqlConnection.Close();
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                mySqlConnection.Close();
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Json("Error");
            }
            finally
            {
                mySqlConnection.Close();
            }
        }
        [HttpPost]
        public IActionResult Accept(int id)
        {
			acceptedOrderID = id;
            return Json("yes");
            //MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
            //try
            //{
               
            //}
            //catch (Exception ex)
            //{
            //    mySqlConnection.Close();
            //    System.Diagnostics.Debug.WriteLine(ex.Message);
            //    return Json("error");
            //}
            //finally
            //{
            //    mySqlConnection.Close();
            //}
        }

        [HttpPost]
        public IActionResult Decline(int id)
        {
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
            try
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCommand = new MySqlCommand("delete from bill where id ='"+ id +"' ;", mySqlConnection);
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
                return Json("success");
            }
            catch (Exception ex)
            {
                mySqlConnection.Close();
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Json("error");
            }
            finally
            {
                mySqlConnection.Close();
            }
        }
		//======================================= Customer Info =============================================
        public IActionResult CustomerInfo()
        {
			string? user = HttpContext.Session.GetString("username");
			if (user == null || string.IsNullOrEmpty(user))
			{
				return RedirectToAction("login", "User", new { area = "" });
			}
            if(acceptedOrderID == -1)
            {
				return RedirectToAction("Worker", "Home", new { area = "" });
			}
			MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
			try
			{
				mySqlConnection.Open();
                //update status
				MySqlCommand mySqlCommand = new MySqlCommand("update bill set status='wait for finish',rider='"+ user + "' where id = '" + acceptedOrderID + "'", mySqlConnection);
                mySqlCommand.ExecuteNonQuery();
				//look up data
				mySqlCommand = new MySqlCommand("select * from bill where id = '"+acceptedOrderID+"'", mySqlConnection);
				MySqlDataReader reader = mySqlCommand.ExecuteReader();
				if (reader.Read())
				{
                    ViewBag.id = acceptedOrderID;
                    ViewBag.foodplace = reader.GetString(1);
					ViewBag.orderDesc = reader.GetString(2);
					ViewBag.customer = reader.GetString(3);
					ViewBag.tel = reader.GetString(4);
					ViewBag.rider = reader.GetString(5);
					ViewBag.place = reader.GetString(6);
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
        public IActionResult finishedOrder()
        {
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
            try
            {
                mySqlConnection.Open();
                //update status
                MySqlCommand mySqlCommand = new MySqlCommand("delete from bill where id = '"+ acceptedOrderID +"' ; ", mySqlConnection);
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
                acceptedOrderID = -1;
                return Json("Success");
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
        }
            //======================================= Menu =============================================
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
		//==================================== Foodlist =============================================
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
            //fooddata = new ConcurrentDictionary<string, Pair<string, int>>();
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
                    food.img_source = reader.GetString(4);

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
        public IActionResult addtocart(string resname, string foodname,string foodprice,string img_source)
        {
            if(shoppingcartdata.Count>=orderlimit)
            {
                return Json("Failed");
            }
            Food food = new Food();
            food.RestaurantName = resname;
            food.Name = foodname;
            food.Price= foodprice;
            food.img_source=img_source;
            shoppingcartdata.TryAdd(idx, food);
            idx++;
            return Json("Success");
        }
		//[HttpPost]
		//public IActionResult Foodlist() //<--- submit to cart
		//{
		//    string? user = HttpContext.Session.GetString("username");
		//    if (user == null || string.IsNullOrEmpty(user))
		//    {
		//        return RedirectToAction("login", "User", new { area = "" });
		//    }
		//    shoppingcartdata.TryAdd(idx, fooddata);
		//    idx++;
		//    fooddata = new ConcurrentDictionary<string, Pair<string, int>>();
		//    return RedirectToAction("ShoppingCart", "Home", new { area = "" });
		//}

		//[HttpPost]
		//public IActionResult IncreaseValueBy(string foodname, string foodprice, int value)
		//{
		//    if (!fooddata.ContainsKey(foodname))
		//    {
		//        fooddata.TryAdd(foodname, new Pair<string, int>(foodprice, 0)); //price, count
		//    }
		//    Pair<string, int> tmp;
		//    fooddata.TryGetValue(foodname, out tmp);
		//    tmp.Second += value;
		//    if (tmp.Second < 0)
		//    {
		//        tmp.Second = 0;
		//        fooddata.TryRemove(foodname, out _);
		//        return Json(0);
		//    }
		//    else
		//    {
		//        fooddata.TryUpdate(foodname, tmp, fooddata[foodname]);
		//        return Json(tmp.Second);
		//    }
		//}

		//==================================== Shopping Cart =============================================
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
        public IActionResult ShoppingCart(billingInfo bill) //submit cart
        {
            if (shoppingcartdata.Count == 0) // filtered case but doesn't use anyway
            {
                return View();
            }
            else {
                MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
                try
                {
                    string desc = "";
                    foreach (var item in shoppingcartdata)
                    {
                        desc += item.Value.RestaurantName + " " + item.Value.Name + " " + item.Value.Price + "\n";
                    }
                    clearShoppingCart();
                    var test = bill.place;
                    mySqlConnection.Open();
                    MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO bill (foodplace, order_desc, customer, customer_Tel,rider, place, status) " +
                                                                        "VALUES ('test','" + bill.place + "','" + desc + "','" + bill.tel + "','none','" + bill.place + "', 'waiting for rider')", mySqlConnection);
                    mySqlCommand.ExecuteNonQuery();
                    mySqlCommand = new MySqlCommand("select LAST_INSERT_ID()",mySqlConnection);
                    MySqlDataReader reader = mySqlCommand.ExecuteReader();
                    if(reader.Read())
                    {
                        acceptedOrderID = reader.GetInt32(0);
                    }
                    else
                    {
                        return View("~/Views/User/Error.cshtml");
                    }
                    mySqlConnection.Close();
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
                return RedirectToAction("Menu", "Finding", new { area = "" });
            }
        }
        //[HttpPost]
        //public IActionResult ShoppingCartIncreaseValueBy(int orderkey, string foodname, int value)
        //{
        //    ConcurrentDictionary<string, Pair<string, int>> tmp;
        //    Pair<string, int> tmpPair;

        //    shoppingcartdata.TryGetValue(orderkey, out tmp);   // get one list of order to tmp
        //    tmp.TryGetValue(foodname, out tmpPair);            // get Pair<cost,count> from tmp to tmpPair

        //    tmpPair.Second += value;
        //    if (tmpPair.Second < 0) { tmpPair.Second = 0; }

        //    tmp.TryUpdate(foodname, tmpPair, tmp[foodname]);  // put back to tmp
        //    shoppingcartdata.TryUpdate(orderkey, tmp, shoppingcartdata[orderkey]); // put back to shoppingcartdata

        //    return Json(tmpPair.Second);
        //}
        [HttpPost]
        public IActionResult deleteOrder(int orderkey)
        {
            shoppingcartdata.TryRemove(orderkey, out _);
            return Json("success");
        }
    }
}
