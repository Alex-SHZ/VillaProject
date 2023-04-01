using System;
using System.ComponentModel.DataAnnotations;

namespace VillaAPI.Models.DTO;

public class VillaNumberDTO
{
    [Required]
    public int VillaNo { get; set; }

    public string Details { get; set; }
}

