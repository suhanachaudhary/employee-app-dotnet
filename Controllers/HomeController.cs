
using Microsoft.AspNetCore.Mvc;
using EmployeeApp.Models;
using MySql.Data.MySqlClient;

namespace EmployeeApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = "Server=localhost;Database=Training;User Id=root;Password=;";

        public IActionResult Index()
        {
            List<Employee> employees = new List<Employee>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM ShilochnaEmployees";
                MySqlCommand cmd = new MySqlCommand(query, con);
                con.Open();
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employees.Add(new Employee
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Pincode = Convert.ToInt32(reader["Pincode"]),
                        Address = reader["Address"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        Email = reader["Email"].ToString(),
                        Photo = reader["Photo"].ToString(),
                        Gender = Convert.ToChar(reader["Gender"]),
                        IAgreed = Convert.ToBoolean(reader["IAgreed"])
                    });
                }
            }

            return View(employees); // 🔥 Now Model will not be null
        }
    }
}
