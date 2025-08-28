
using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeApp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string Name { get; set; }

        [Required, Range(100000, 999999)]
        public int Pincode { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required, StringLength(13)]
        [RegularExpression(@"^\d{2}-\d{10}$", ErrorMessage = "Format: XX-XXXXXXXXXX")]
        public string PhoneNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Photo { get; set; }   // store file path

        [Required]
        public char Gender { get; set; }  // M/F

        public bool IAgreed { get; set; }
    }
}
