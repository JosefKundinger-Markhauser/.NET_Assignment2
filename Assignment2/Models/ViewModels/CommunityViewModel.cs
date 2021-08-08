using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Models.ViewModels
{
    public class CommunityViewModel
    {

        public IEnumerable<Community> Communities
        {
            get;
            set;
        }

        public IEnumerable<Student> Students
        {
            get;
            set;
        }
    }
}
