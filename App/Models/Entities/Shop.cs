using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Entities
{
public class Shop
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "varchar(256)")]
    public string Name { get; set; }

    [Required]
    [Column(TypeName = "int(11)")]
    public int AgencyId { get; set; }

    [Required]
    [Column(TypeName = "int(11)")]
    public int RegionalCompanyId { get; set; }

    [Required]
    [Column(TypeName = "int(11)")]
    public int BranchId { get; set; }

    [Required]
    [Column(TypeName = "int(11)")]
    public int ShopUserId { get; set; }

    [Required]
    [Column(TypeName = "int(11)")]
    public int StaffUserId { get; set; }
}
}
