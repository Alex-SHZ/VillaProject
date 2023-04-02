using System;
using System.ComponentModel.DataAnnotations;

namespace VillaWeb.Models.DTO;

public class VillaNumberUpdateDTO
{
    [Required]
    public int VillaNo { get; set; }
    [Required]
    public int VillaId { get; set; }

    public string Details { get; set; }
}

