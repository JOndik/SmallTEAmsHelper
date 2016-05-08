using System;
using System.Data.SQLite;
using Microsoft.Data.Entity;
using BPAddIn.DataContract;

namespace BPAddIn
{
    public class LocalDBContext : DbContext
    {
        public static object Lock = new object();
        public DbSet<Word> dictionary { get; set; }
        public DbSet<ModelChange> modelChanges { get; set; }
        public DbSet<PropertyChange> propertyChanges { get; set; }
        public DbSet<ItemCreation> itemCreations { get; set; }
        public DbSet<ScenarioChange> scenarioChanges { get; set; }
        public DbSet<StepChange> stepChanges { get; set; }

        public DbSet<DefectReport> defectReports { get; set; }

        public DbSet<User> user { get; set; }

        public DbSet<Version> version { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));
            string programData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            var connection = new SQLiteConnection("Data Source=" + programData + "\\sthAddIn\\db.sqlite;Version=3;UTF8Encoding=True;");         
            optionsBuilder.UseSqlite(connection);
        }
    }
}
