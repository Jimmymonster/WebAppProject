using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TestProject.Models;

namespace TestProject.Controllers
{
    public class UserController : Controller
    {
        string mysqlCon = "Server=85.10.205.173;" +
            "Port=3306;" +
            "database=rubsarbrurueng;" +
            "user=a_d_m_i_n;" +
            "password=adminadmin;";
        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            TempData.Clear();
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
            try
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCommand = new MySqlCommand("select * from `user` where Name='" + user.Username + "'and Password='" + user.Password + "' COLLATE utf8mb4_0900_as_cs", mySqlConnection);
                MySqlDataReader reader = mySqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    mySqlConnection.Close();
                    HttpContext.Session.SetString("username", user.Username ?? "Error Username is NULL");
                    return RedirectToAction("Menu", "Home", new { area = "" });
                }
                else
                {
                    mySqlConnection.Close();
                    return RedirectToAction("loginfailed", "User", new { area = "" });
                }
            }
            catch (Exception ex)
            {
                mySqlConnection.Close();
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View("Error");
            }
            finally
            {
                mySqlConnection.Close();
            }
        }
        public IActionResult loginfailed()
        {
            HttpContext.Session.Clear();
            TempData.Clear();
            return View();
        }
        [HttpPost]
        public IActionResult loginfailed(User user)
        {
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
            try
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCommand = new MySqlCommand("select * from `user` where Name='" + user.Username + "'and Password='" + user.Password + "' COLLATE utf8mb4_0900_as_cs", mySqlConnection);
                MySqlDataReader reader = mySqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    mySqlConnection.Close();
                    HttpContext.Session.SetString("username", user.Username ?? "Error Username is NULL");
                    return RedirectToAction("Menu", "Home", new { area = "" });
                }
                else
                {
                    mySqlConnection.Close();
                    return RedirectToAction("loginfailed", "User", new { area = "" });
                }
            }
            catch (Exception ex)
            {
                mySqlConnection.Close();
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return View("Error");
            }
            finally
            {
                mySqlConnection.Close();
            }
        }
        
    }
}
