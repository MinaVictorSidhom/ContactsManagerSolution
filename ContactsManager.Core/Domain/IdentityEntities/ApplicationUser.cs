using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsManager.Core.Domain.IdentityUser
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? PersonName { get; set; }

    }
}
