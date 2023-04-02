using System;
using System.ComponentModel.DataAnnotations;

namespace VillaWeb.Models.DTO;

public class VillaNumberDTO
{
    [Required]
    public int VillaNo { get; set; }
    [Required]
    public int VillaId { get; set; }

    public string Details { get; set; }
    public VillaDTO Villa { get; set; }
}

