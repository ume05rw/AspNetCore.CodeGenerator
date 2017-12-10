using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using App.Models.DbContexts;

namespace App.Migrations
{
    [DbContext(typeof(Db))]
    partial class DbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.4");

            modelBuilder.Entity("App.Models.Entities.Shop", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)");

                    b.Property<int>("AgencyId")
                        .HasColumnType("int(11)");

                    b.Property<int>("BranchId")
                        .HasColumnType("int(11)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(256)");

                    b.Property<int>("RegionalCompanyId")
                        .HasColumnType("int(11)");

                    b.Property<int>("ShopUserId")
                        .HasColumnType("int(11)");

                    b.Property<int>("StaffUserId")
                        .HasColumnType("int(11)");

                    b.HasKey("Id");

                    b.ToTable("Shops");
                });
        }
    }
}
