using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace Homework.Data.DataAnnotaions
{
    // Reference : https://stackoverflow.com/questions/56588900/how-to-validate-uploaded-file-in-asp-net-core
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string _ErrorMessage;
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string extensionsStr, string ErrorMessage)
        {
            _extensions = extensionsStr.Split(",");
            _ErrorMessage = ErrorMessage;
        }

        public string GetErrorMessage()
        {
            return string.IsNullOrWhiteSpace(_ErrorMessage) ? $"This photo extension is not allowed!" : _ErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }
    }
}