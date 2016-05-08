using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using BPAddIn;

namespace BPAddIn.Migrations
{
    [DbContext(typeof(LocalDBContext))]
    [Migration("20151109223017_schemaMigration4")]
    partial class schemaMigration4
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Annotation("ProductVersion", "7.0.0-beta8-15964");

            modelBuilder.Entity("BPAddIn.DataContract.ModelChange", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("timestamp");

                    b.Property<string>("userGUID");

                    b.HasKey("id");

                    b.Annotation("Relational:TableName", "model_changes");
                });

            modelBuilder.Entity("BPAddIn.DataContract.PropertyChange", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("elementGUID");

                    b.Property<string>("propertyBody");

                    b.Property<int>("propertyType");

                    b.Property<string>("timestamp");

                    b.Property<string>("userGUID");

                    b.HasKey("id");

                    b.Annotation("Relational:TableName", "property_changes");
                });

            modelBuilder.Entity("BPAddIn.Word", b =>
                {
                    b.Property<string>("word")
                        .Annotation("Relational:ColumnName", "word");

                    b.Property<string>("flag")
                        .Annotation("Relational:ColumnName", "flags");

                    b.Property<string>("wordBase")
                        .Annotation("Relational:ColumnName", "base");

                    b.HasKey("word");

                    b.Annotation("Relational:TableName", "dictionary");
                });
        }
    }
}
