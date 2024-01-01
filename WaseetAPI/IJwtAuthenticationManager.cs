using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaseetAPI.Domain.Models;

namespace WaseetAPI
{
    public interface IJwtAuthenticationManager
    {
        string Authenticate(string tokenValue);
        string ValidateJwtToken(string token);
    }
}
