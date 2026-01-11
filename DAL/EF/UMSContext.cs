using DAL.EF.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF
{
    public class UMSContext: DbContext
    {
        public UMSContext(DbContextOptions<UMSContext> opt) : base(opt)
        {
        }
        public DbSet<User> Users { get; set; }
    
    }
}
