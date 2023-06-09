﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Models.DTO;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers.v1;

[Route("api/v{version:apiVersion}/VillaAPI")]
[ApiController]
[ApiVersion("1.0")]
public class VillaAPIController : ControllerBase
{
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;
    private readonly ILogger<VillaAPIController> _logger;
    protected APIResponse _responce;

    public VillaAPIController(
        ILogger<VillaAPIController> logger,
        IVillaRepository dbVilla,
        IMapper mapper)
    {
        _logger = logger;
        _dbVilla = dbVilla;
        _mapper = mapper;
        _responce = new();
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ResponseCache(CacheProfileName = "Default30")]
    public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "filterOccupancy")] int? occupancy,
        [FromQuery] string? search, int pageSize = 0, int pageNumber = 1)
    {
        try
        {
            IEnumerable<Villa> villaList;

            if (occupancy > 0)
                villaList = await _dbVilla.GetAllAsync(u => u.Occupancy == occupancy, pageSize: pageSize, pageNumber: pageNumber);
            else
                villaList = await _dbVilla.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

            if (!string.IsNullOrEmpty(search))
                villaList = villaList.Where(u => u.Name.ToLower().Contains(search));

            Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

            _responce.Result = _mapper.Map<List<VillaDTO>>(villaList);
            _responce.StatucCode = HttpStatusCode.OK;
            return Ok(_responce);

        }
        catch (Exception ex)
        {
            _responce.IsSucces = false;
            _responce.ErrorMessages =
                new List<string>() { ex.ToString() };
        }
        return (_responce);
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> GetVilla(int id)
    {
        try
        {
            if (id == 0)
            {
                _responce.StatucCode = HttpStatusCode.BadRequest;
                return BadRequest(_responce);
            }

            Villa villa = await _dbVilla.GetAsync(u => u.Id == id);

            if (villa == null)
            {
                _responce.StatucCode = HttpStatusCode.NotFound;
                return NotFound(_responce);
            }

            _responce.Result = _mapper.Map<VillaDTO>(villa);
            _responce.StatucCode = HttpStatusCode.OK;
            return Ok(_responce);
        }
        catch (Exception ex)
        {
            _responce.IsSucces = false;
            _responce.ErrorMessages =
                new List<string>() { ex.ToString() };
        }
        return (_responce);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
    {
        try
        {
            if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("ErrorMessages", "Villa already Exists!");
                return BadRequest(ModelState);
            }

            if (createDTO == null)
            {
                _responce.StatucCode = HttpStatusCode.BadRequest;
                _responce.Result = createDTO;
                return BadRequest(_responce);
            }


            Villa villa = _mapper.Map<Villa>(createDTO);

            await _dbVilla.CreateAsync(villa);

            _responce.Result = _mapper.Map<VillaDTO>(villa);
            _responce.StatucCode = HttpStatusCode.Created;

            return CreatedAtRoute("GetVilla", new { id = villa.Id }, _responce);
        }
        catch (Exception ex)
        {
            _responce.IsSucces = false;
            _responce.ErrorMessages =
                new List<string>() { ex.ToString() };
        }
        return (_responce);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest();
            }


            var villa = await _dbVilla.GetAsync(u => u.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            await _dbVilla.RemoveAsync(villa);

            _responce.StatucCode = HttpStatusCode.OK;

            return Ok(_responce);
        }
        catch (Exception ex)
        {
            _responce.IsSucces = false;
            _responce.ErrorMessages =
                new List<string>() { ex.ToString() };
        }
        return (_responce);

    }

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
    {
        try
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }

            Villa model = _mapper.Map<Villa>(updateDTO);

            await _dbVilla.UpdateAsync(model);

            _responce.StatucCode = HttpStatusCode.NoContent;

            return Ok(_responce);
        }
        catch (Exception ex)
        {
            _responce.IsSucces = false;
            _responce.ErrorMessages =
                new List<string>() { ex.ToString() };
        }
        return _responce;
    }

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
    {
        if (patchDTO == null || id == 0)
            return BadRequest();

        var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

        VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);


        if (villa == null)
            return BadRequest();


        patchDTO.ApplyTo(villaDTO, ModelState);
        Villa model = _mapper.Map<Villa>(villaDTO);

        await _dbVilla.UpdateAsync(model);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return NoContent();
    }


}

