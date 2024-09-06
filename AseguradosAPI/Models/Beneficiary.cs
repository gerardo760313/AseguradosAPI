using AseguradosAPI.Utilities;
using System.ComponentModel.DataAnnotations;

namespace AseguradosAPI.Models
{
    public class Beneficiary
    {
        public int BeneficiaryId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [CustomValidation(typeof(ValidationHelpers), nameof(ValidationHelpers.ValidateMayorDeEdad))]
        public DateTime BirthDate { get; set; }
        [Required]
        public string CURP { get; set; }
        [Required]
        public string SSN { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Nationality { get; set; }
        [Required]
        public decimal ParticipationPercentage { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }

    public class EditBeneficiariesViewModel
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }       
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string CURP { get; set; }       
        public string SSN { get; set; }        
        public string Phone { get; set; }       
        public string Nationality { get; set; }        
        public decimal ParticipationPercentage { get; set; }
        public List<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();  // Inicializa como lista vacía
    }
}
