using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using VillaWeb.Models;
using VillaWeb.Models.DTO;
using VillaWeb.Models.VM;
using VillaWeb.Services;
using VillaWeb.Services.IServices;

namespace VillaWeb.Controllers;

public class VillaNumberController : Controller
{
    private readonly IVillaNumberService _villaNumberService;
    private readonly IVillaService _villaService;
    private readonly IMapper _mapper;
    public VillaNumberController(
        IVillaNumberService villaNumberService,
        IMapper mapper,
        IVillaService villaService)
    {
        _villaService = villaService;
        _villaNumberService = villaNumberService;
        _mapper = mapper;
    }

    public async Task<IActionResult> IndexVillaNumber()
    {
        List<VillaNumberDTO> list = new();

        var response = await _villaNumberService.GetAllAsync<APIResponse>();
        if (response != null && response.IsSucces)
        {
            list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
        }
        return View(list);
    }

    public async Task<IActionResult> CreateVillaNumber()
    {
        VillaNumberCreateVM villaNumberCreateVM = new();
        var response = await _villaService.GetAllAsync<APIResponse>();
        if (response != null && response.IsSucces)
        {
            villaNumberCreateVM.VillaList
                = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result))
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
        }
        return View(villaNumberCreateVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
    {
        if (ModelState.IsValid)
        {

            APIResponse response = await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumber);
            if (response != null && response.IsSucces)
            {
                return RedirectToAction(nameof(IndexVillaNumber));
            }
        }
        return View(model);
    }

    //public async Task<IActionResult> UpdateVillaNumber(int villaId)
    //{
    //    APIResponse response = await _villaService.GetAsync<APIResponse>(villaId);
    //    if (response != null && response.IsSucces)
    //    {
    //        VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
    //        return View(_mapper.Map<VillaUpdateDTO>(model));
    //    }
    //    return NotFound();
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateDTO model)
    //{
    //    if (ModelState.IsValid)
    //    {

    //        APIResponse response = await _villaService.UpdateAsync<APIResponse>(model);
    //        if (response != null && response.IsSucces)
    //        {
    //            return RedirectToAction(nameof(IndexVilla));
    //        }
    //    }
    //    return View(model);
    //}

    //public async Task<IActionResult> DeleteVillaNumber(int villaId)
    //{
    //    APIResponse response = await _villaService.GetAsync<APIResponse>(villaId);
    //    if (response != null && response.IsSucces)
    //    {
    //        VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
    //        return View(model);
    //    }
    //    return NotFound();
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteVillaNumber(VillaNumberDTO model)
    //{

    //    APIResponse response = await _villaService.DeleteAsync<APIResponse>(model.Id);
    //    if (response != null && response.IsSucces)
    //    {
    //        return RedirectToAction(nameof(IndexVilla));
    //    }

    //    return View(model);
    //}
}

