using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Utility
{
    
    public class FileUploadOperation : IOperationFilter
    {
        
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            var isFileUploadOperation =
                context.MethodInfo.CustomAttributes.Any(a => a.AttributeType == typeof(FileContentType));

            if (!isFileUploadOperation) return;

            operation.Parameters.Clear();

            var uploadFileMediaType = new OpenApiMediaType()
            {
                Schema = new OpenApiSchema()
                {
                    Type = "object",
                    Properties =
                {
                    ["uploadedFile"] = new OpenApiSchema()
                    {
                        Description = "Upload File",
                        Type = "file",
                        Format = "formData"
                    }
                },
                    Required = new HashSet<string>() { "uploadedFile" }
                }
            };

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = { ["multipart/form-data"] = uploadFileMediaType }
            };
        }

        
        [AttributeUsage(AttributeTargets.Method)]
        public class FileContentType : Attribute
        {

        }
    }
}
