using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace TraceSystem.Context
{
    public class HistoryContext:DbContext
    {
        public HistoryContext(DbContextOptions<HistoryContext> options) : base(options)
        {

        }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlite("Data Source=Hist.db");
        }
        public DbSet<ItemValues> ItemValuestb { get; set; }
        public DbSet<ItemNames> ItemNamestb { get; set; }
    }


    public class DbContextDesignTimeFactory : IDesignTimeDbContextFactory<HistoryContext>
    {
        public HistoryContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<HistoryContext> builder = new DbContextOptionsBuilder<HistoryContext>();
            builder.UseSqlite("Data Source=Hist.db");
            return new HistoryContext(builder.Options);


        }
    }
}
