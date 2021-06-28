using System;
using System.IO;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProxyAPI.Security
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class FileUploadSecurityValidation : ValidationAttribute
    {
        private static readonly int kiloByte = 1000;
        private static readonly int _maxFileSizeBytes = 10 * kiloByte;
        private static readonly int _minFileSizeBytes = 1;
        private static readonly string[] _supportedFileTypes = new string[] { string.Empty, ".txt" };
        private static readonly string[] _supportedContentTypes = new string[] { "text/plain" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult? result = new ValidationResult("Invalid File");

            try
            {
                if (value is IFormFile)
                {
                    IFormFile file = (IFormFile)value;
                    if (file != null)
                    {
                        // Check the size of an uploaded file. Set a maximum size limit to prevent large uploads.
                        if (file.Length <= 0 || file.Length > _maxFileSizeBytes)
                        {
                            throw new ArgumentException($"Error: Invalid File Size. minBytes={_minFileSizeBytes}, maxBytes={_maxFileSizeBytes}, uploadFileBytes{file.Length}");
                        }

                        // Allow only approved file extensions for the app's design specification.
                        string fileExt = Path.GetExtension(file.Name).ToLowerInvariant();
                        if (!_supportedFileTypes.Contains(fileExt))
                        {
                            throw new ArgumentException($"Error: Invalid File ContentType.");
                        }

                        // Allow only approved file content types for the app's design specification.
                        if (file.ContentType == null || !_supportedContentTypes.Contains(file.ContentType))
                        {
                            throw new ArgumentException($"Error: Invalid File ContentType.");
                        }

                        /*
                         * Some additional security concerns that should be imployed to ensure proper security validation here + in app environment:
                         *  - Disable execute permissions on the file upload location.
                         *  - Upload files to a dedicated file upload area, preferably to a non-system drive.
                         *  - Do not persist uploaded files in the same directory tree as the app.
                         *  - Don't use a file name provided by the user or the untrusted file name of the uploaded file.
                         *  - HTML encode the untrusted file name when logging it, or displaying in UI.
                         *  - Verify that client-side checks are performed on the server.
                         *  - Verify content does not contain a virus or malware before the file is stored.
                         *  - When storing a file on a physical storage (file system or network share) this application that is accessing the file 
                         *      must have read/write permissions to the storage location, but never grand execute permissions.
                         */

                        result = ValidationResult.Success;
                    }
                }
            }
            catch (ArgumentException ae)
            {
                result = new ValidationResult(ae.Message);
            }
            catch (Exception)
            {
                result = new ValidationResult("Error Validating File");
            }

            return result;
        }
    }
}
