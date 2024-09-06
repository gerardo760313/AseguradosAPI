using System;
using System.ComponentModel.DataAnnotations;

namespace AseguradosAPI.Utilities
{
    public class ValidationHelpers
    {
        public static ValidationResult ValidateMayorDeEdad(DateTime fechaNacimiento, ValidationContext context)
        {
            int edad = DateTime.Today.Year - fechaNacimiento.Year;
            if (fechaNacimiento > DateTime.Today.AddYears(-edad))
            {
                edad--;
            }
            return edad >= 18 ? ValidationResult.Success : new ValidationResult("Debe ser mayor de edad");
        }


    }
}
