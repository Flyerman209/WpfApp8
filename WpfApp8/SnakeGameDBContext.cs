using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace WpfApp8
{
    public class SnakeGameDBContext : DbContext
    {
        public DbSet<HighScore> HighScores { get; set; }

        public SnakeGameDBContext() : base("name=SnakeGameDBConnection")
        {
        }



    }
}
