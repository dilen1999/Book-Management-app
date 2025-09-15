using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Auth.Dtos
{
    public sealed class GoogleSignInRequest
    {
        public string IdToken { get; set; } = string.Empty;
    }
}
