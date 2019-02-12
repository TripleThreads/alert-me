using AlertMe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertMe.ViewModels
{
    public class StateUserViewModel
    {
        public IEnumerable<State> States { get; set; }
        public User User { get; set; }
    }
}
