using Homework.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homework.DAL
{
    public class BlogDbContext : DbContext
    {
        //enable-migrations
        //add-migration Init
        //update-database

        /*
         * P.S. 不要手賤用指令裝最新的會裝到5
         * 安裝套件整理(version 3.1.8)
         * 1. Microsoft.EntityFrameworkCore
         * 2. Microsoft.EntityFrameworkCore.Abstractions
         * 3. Microsoft.EntityFrameworkCore.Design
         * 4. Microsoft.EntityFrameworkCore.Relational
         * 5. Microsoft.EntityFrameworkCore.SqlServer
         * 6. Microsoft.EntityFrameworkCore.Tools
         */

        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Articles> Articles { get; set; }
        public virtual DbSet<TagCloud> TagCloud { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ValueGeneratedNever 不使用預設產生ID
            modelBuilder.Entity<Articles>(entity => { entity.Property(e => e.Id).ValueGeneratedNever(); });
            modelBuilder.Entity<TagCloud>(entity => { entity.Property(e => e.Id).ValueGeneratedNever(); });
        }
    }
}