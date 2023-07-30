using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModel
{
    public class AccountView
    {
        [BindNever]
        public string Name { get; set; } = null;
        public string Password { get; set; } = null;
    }
}
