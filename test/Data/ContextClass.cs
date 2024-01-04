using Microsoft.EntityFrameworkCore;

namespace test.Data
{
    public class ContextClass : DbContext
    {
        public ContextClass(DbContextOptions options):base(options) { }
        
    }
}
