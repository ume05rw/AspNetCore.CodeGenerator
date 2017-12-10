using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Data;
using App.Models.Entities;

namespace App.Models.DbContexts
{
    public class Db : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Shop> Shops { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public Db(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// Create DbSets
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
        }
    }
}
