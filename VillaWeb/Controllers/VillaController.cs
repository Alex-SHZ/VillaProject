using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VillaWeb.Models;
using VillaWeb.Models.DTO;
using VillaWeb.Services.IServices;

namespace VillaWeb.Controllers;

public class VillaController : Controller
{
    private readonly IVillaService _villaService;
    private readonly IMapper _mapper;

    public VillaController(IVillaService villaService, IMapper mapper)
    {
        _villaService = villaService;
        _mapper = mapper;
    }

    public async Task<IActionResult> IndexVilla()
    {
        List<VillaDTO> list = new();

        APIResponse response = await _villaService.GetAllAsync<APIResponse>();

        if (response != null && response.IsSucces)
        {
            list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
        }

        return View(list);
    }

    public async Task<IActionResult> CreateVilla()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVilla(VillaCreateDTO model)
    {
        if (ModelState.IsValid)
        {

            APIResponse response = await _villaService.CreateAsync<APIResponse>(model);
            if (response != null && response.IsSucces)
            {
                TempData["success"] = "Villa created successfully";
                return RedirectToAction(nameof(IndexVilla));
            }
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }

    public async Task<IActionResult> UpdateVilla(int villaId)
    {
        APIResponse response = await _villaService.GetAsync<APIResponse>(villaId);
        if (response != null && response.IsSucces)
        {
            VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
            return View(_mapper.Map<VillaUpdateDTO>(model));
        }
        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateVilla(VillaUpdateDTO model)
    {
        if (ModelState.IsValid)
        {

            APIResponse response = await _villaService.UpdateAsync<APIResponse>(model);
            if (response != null && response.IsSucces)
            {
                TempData["success"] = "Villa updated successfully";
                return RedirectToAction(nameof(IndexVilla));
            }
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }

    public async Task<IActionResult> DeleteVilla(int villaId)
    {
        APIResponse response = await _villaService.GetAsync<APIResponse>(villaId);
        if (response != null && response.IsSucces)
        {
            VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
            return View(model);
        }
        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVilla(VillaDTO model)
    {

        APIResponse response = await _villaService.DeleteAsync<APIResponse>(model.Id);
        if (response != null && response.IsSucces)
        {
            TempData["success"] = "Villa deleted successfully";
            return RedirectToAction(nameof(IndexVilla));
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }
}

