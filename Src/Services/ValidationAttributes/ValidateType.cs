using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FinancialEntries.Services.Store
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateValue : ValidationAttribute
    {
        private Type _validationType;

        public ValidateValue(Type validationType)
        {
            _validationType = validationType;
        }

        protected override ValidationResult IsValid(
            object value, 
            ValidationContext validationContext)
        {
            string input = value as string;

            if (input == null) 
                return new ValidationResult("Property must be string to be validated");

            MethodInfo[] methods = _validationType
                .GetMethods(BindingFlags.Public | BindingFlags.Static);
            string possibleValues = "";
            bool validInput = false;

            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name == input)
                {
                    validInput = true;
                    break;
                }

                possibleValues += $"{methods[i].Name}, ";
            }

            if (!validInput)
            {
                return new ValidationResult($"Not inside possible values: " +
                    $"{possibleValues.Remove(possibleValues.LastIndexOf(","), 2)}");
            }

            return ValidationResult.Success;
        }
    }
}
