using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Data
{
    public class DbInitializer
    {

        public static void Initialize(SchoolCommunityAdsContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
