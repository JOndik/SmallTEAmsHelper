﻿using System;
using System.Data.SQLite;
using Microsoft.Data.Entity;
using BPAddIn.DataContract;

namespace BPAddIn
{
    public class LocalDBContext : DbContext
    {
        public DbSet<Word> dictionary { get; set; }
        public DbSet<ModelChange> modelChanges { get; set; }

        public DbSet<PropertyChange> propertyChanges { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            var connection = new SQLiteConnection("Data Source=|DataDirectory|db.sqlite;Version=3;");
            optionsBuilder.UseSqlite(connection);
        }
    }
}
