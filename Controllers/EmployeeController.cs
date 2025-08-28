
using Microsoft.AspNetCore.Mvc;
using EmployeeApp.Models;
using MySql.Data.MySqlClient;

namespace EmployeeApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly string connectionString = "Server=localhost;Port=3306;Database=Training;Uid=root;Pwd=;";

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

            return View(employees);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee emp, IFormFile photoFile)
        {
            string photoPath = null;

            try
            {
                // Ensure uploads folder exists
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                // Save file if uploaded
                if (photoFile != null && photoFile.Length > 0)
                {
                    photoPath = Path.Combine("/uploads", photoFile.FileName);
                    using (var stream = new FileStream(Path.Combine(uploads, photoFile.FileName), FileMode.Create))
                    {
                        photoFile.CopyTo(stream);
                    }
                }

                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    string query = @"INSERT INTO ShilochnaEmployees 
                     (Name, Pincode, Address, DOB, PhoneNumber, Email, Photo, Gender, IAgreed) 
                     VALUES (@Name, @Pincode, @Address, @DOB, @PhoneNumber, @Email, @Photo, @Gender, @IAgreed)";

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Name", emp.Name);
                    cmd.Parameters.AddWithValue("@Pincode", emp.Pincode);
                    cmd.Parameters.AddWithValue("@Address", emp.Address);
                    cmd.Parameters.AddWithValue("@DOB", emp.DOB);
                    cmd.Parameters.AddWithValue("@PhoneNumber", emp.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                    cmd.Parameters.AddWithValue("@Photo", photoPath ?? "");
                    cmd.Parameters.AddWithValue("@Gender", emp.Gender);
                    cmd.Parameters.AddWithValue("@IAgreed", emp.IAgreed);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    con.Close();

                    if (rows > 0)
                    {
                        TempData["Success"] = "Employee created successfully!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Error"] = "Insert failed, no rows affected.";
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Insert failed: " + ex.Message;
                return View(emp);
            }

            return RedirectToAction("Index", "Home");
        }

        // ----------------- EDIT EMPLOYEE -----------------
        public IActionResult Edit(int id)
        {
            Employee emp = new Employee();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM ShilochnaEmployees WHERE Id=@Id";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    emp.Id = Convert.ToInt32(reader["Id"]);
                    emp.Name = reader["Name"].ToString();
                    emp.Pincode = Convert.ToInt32(reader["Pincode"]);
                    emp.Address = reader["Address"].ToString();
                    emp.DOB = Convert.ToDateTime(reader["DOB"]);
                    emp.PhoneNumber = reader["PhoneNumber"].ToString();
                    emp.Email = reader["Email"].ToString();
                    emp.Photo = reader["Photo"].ToString();
                    emp.Gender = Convert.ToChar(reader["Gender"]);
                    emp.IAgreed = Convert.ToBoolean(reader["IAgreed"]);
                }
            }

            return View(emp);

        }

        [HttpPost]
        public IActionResult Edit(Employee emp, IFormFile? photoFile)
        {
            string photoPath = emp.Photo; // keep old photo if not updated
            if (photoFile != null)
            {
                photoPath = Path.Combine("wwwroot/uploads", photoFile.FileName);
                using (var stream = new FileStream(photoPath, FileMode.Create))
                {
                    photoFile.CopyTo(stream);
                }
            }

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                string query = "UPDATE ShilochnaEmployees SET Name=@Name, Pincode=@Pincode, Address=@Address, DOB=@DOB, PhoneNumber=@PhoneNumber, Email=@Email, Photo=@Photo, Gender=@Gender, IAgreed=@IAgreed WHERE Id=@Id";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", emp.Id);
                cmd.Parameters.AddWithValue("@Name", emp.Name);
                cmd.Parameters.AddWithValue("@Pincode", emp.Pincode);
                cmd.Parameters.AddWithValue("@Address", emp.Address);
                cmd.Parameters.AddWithValue("@DOB", emp.DOB);
                cmd.Parameters.AddWithValue("@PhoneNumber", emp.PhoneNumber);
                cmd.Parameters.AddWithValue("@Email", emp.Email);
                cmd.Parameters.AddWithValue("@Photo", photoPath);
                cmd.Parameters.AddWithValue("@Gender", emp.Gender);
                cmd.Parameters.AddWithValue("@IAgreed", emp.IAgreed);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = $"User {emp.Name} updated successfully!!";
            return RedirectToAction("Index", "Home");
        }

        // ----------------- DELETE EMPLOYEE -----------------
        public IActionResult Delete(int id)
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                string query = "DELETE FROM ShilochnaEmployees WHERE Id=@Id";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "Employee deleted successfully!!";
            return RedirectToAction("Index", "Home");
        }
    }
}
