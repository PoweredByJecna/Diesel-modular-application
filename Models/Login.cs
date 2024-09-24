using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Diesel_modular_application.Controllers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace Diesel_modular_application.Models
{
    public class Login
    {
        public required InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            public required string UserName { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public required string Password { get; set; }
            public bool RememberMe { get; set; }
        }
    }
}
