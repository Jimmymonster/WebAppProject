using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NuGet.Protocol.Plugins;
using System.Diagnostics.Contracts;
using TestProject.Models;

namespace TestProject.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User user)
        {
            string mysqlCon = "Server=85.10.205.173;" +
            "Port=3306;" +
            "database=rubsarbrurueng;" +
            "user=a_d_m_i_n;" +
            "password=adminadmin;";
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
            try
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCommand = new MySqlCommand("select * from `user` where Name='" + user.Username + "'and Password='" + user.Password + "' COLLATE utf8mb4_0900_as_cs", mySqlConnection);
                MySqlDataReader reader = mySqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    String ID = reader.GetString(0);
                    String Name = reader.GetString(1);
                    String Pass = reader.GetString(2);
                    String Status = reader.GetString(3);
                    mySqlConnection.Close();
                    return View("Menu");
                }
                else
                {
                    mySqlConnection.Close();
                    return View("loginfailed");
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
        [HttpGet]
        public ActionResult loginfailed()
        {
            return View();
        }
        [HttpPost]
        public ActionResult loginfailed(User user)
        {
            string mysqlCon = "Server=85.10.205.173;" +
            "Port=3306;" +
            "database=rubsarbrurueng;" +
            "user=a_d_m_i_n;" +
            "password=adminadmin;";
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon);
            try
            {
                mySqlConnection.Open();
                MySqlCommand mySqlCommand = new MySqlCommand("select * from `user` where Name='" + user.Username + "'and Password='" + user.Password + "' COLLATE utf8mb4_0900_as_cs", mySqlConnection);
                MySqlDataReader reader = mySqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    String ID = reader.GetString(0);
                    String Name = reader.GetString(1);
                    String Pass = reader.GetString(2);
                    String Status = reader.GetString(3);
                    mySqlConnection.Close();
                    return View("Menu");
                }
                else
                {
                    mySqlConnection.Close();
                    return View("Error");
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
