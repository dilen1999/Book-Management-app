using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Common.Options
{
    public class GoogleAuthOptions
    {
        // one or more Google OAuth client IDs that are allowed to sign in
        public string[] ClientIds { get; set; } = System.Array.Empty<string>();
    }
}
