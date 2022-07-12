using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Domain.Entities.Abstract;

namespace Todos.Domain.Entities
{
    public class User : RootEntity
    {
        public string Email { get; set; } = "";

        public string Password { get; set; } = "";
    }
}
