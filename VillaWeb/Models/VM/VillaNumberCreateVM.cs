using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using VillaWeb.Models.DTO;

namespace VillaWeb.Models.VM;

public class VillaNumberCreateVM
{
    public VillaNumberCreateVM()
    {
        VillaNumber = new VillaNumberCreateDTO();
    }
    public VillaNumberCreateDTO VillaNumber { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> VillaList { get; set; }
}

