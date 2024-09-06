using AseguradosAPI.Utilities;
using System.ComponentModel.DataAnnotations;

namespace AseguradosAPI.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [CustomValidation(typeof(ValidationHelpers), nameof(ValidationHelpers.ValidateMayorDeEdad))]
        public DateTime BirthDate { get; set; }
        [Required]
        public string EmployeeNumber { get; set; }
        [Required]
        public string CURP { get; set; }
        [Required]
        public string SSN { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Nationality { get; set; }
        public ICollection<Beneficiary>? Beneficiaries { get; set; }
    }
}
