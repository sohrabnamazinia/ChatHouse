using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SAM_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Services
{
    public interface IJWTService
    {
        public string GenerateToken(AppUser user);
        Task<AppUser> FindUserByTokenAsync(HttpRequest request, AppDbContext context);
    }
}
