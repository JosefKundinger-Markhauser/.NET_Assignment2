using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Models.ViewModels
{
    public class DeleteViewModel
    {
        public Community Community
        {
            get;
            set;
        }

        public bool CanDelete
        {
            get;
            set;
        }
    }
}
