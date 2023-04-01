﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Models.DTO;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers;

[Route("api/VillaNumberAPI")]
[ApiController]
public class VillaNumberAPIController : ControllerBase
{
    private readonly IVillaNumberRepository _dbVillaNumber;
    private readonly IMapper _mapper;
    private readonly ILogger<VillaAPIController> _logger;
    protected APIResponse _responce;

    public VillaNumberAPIController(
        ILogger<VillaAPIController> logger,
        IVillaNumberRepository dbVilla,
        IMapper mapper)
    {
        _logger = logger;
        _dbVillaNumber = dbVilla;
        _mapper = mapper;
        _responce = new();
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse>> GetVillaNumbers()
    {
        try
        {
            IEnumerable<VillaNumber> villaNumbersList = await _dbVillaNumber.GetAllAsync();
            _responce.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbersList);
            _responce.StatucCode = HttpStatusCode.OK;
            return Ok(_responce);
        }
        catch(Exception ex)
        {
            _responce.IsSucces = false;
            _responce.ErrorMessages =
                new List<string>() { ex.ToString() };
        }
        return (_responce);
    }

    [HttpGet("{id:int}", Name = "GetVillaNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
    {
        try
        {
            if (id == 0)
            {
                _responce.StatucCode = HttpStatusCode.BadRequest;
                return BadRequest(_responce);
            }

            var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);

            if (villaNumber == null)
            {
                _responce.StatucCode = HttpStatusCode.NotFound;
                return NotFound(_responce);
            }

            _responce.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
    {
        try
        {
            if (await _dbVillaNumber.GetAsync(u => u.VillaNo == createDTO.VillaNo) != null)
            {
                _responce.StatucCode = HttpStatusCode.BadRequest;
                _responce.Result =
                    new List<string>() { "CustomError", "Villa number already Exists!" };
                return BadRequest(_responce);
            }

            if (createDTO == null)
            {
                _responce.StatucCode = HttpStatusCode.BadRequest;
                _responce.Result = createDTO;
                return BadRequest(_responce);
            }


            VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);

            await _dbVillaNumber.CreateAsync(villaNumber);

            _responce.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
            _responce.StatucCode = HttpStatusCode.Created;

            return CreatedAtRoute("GetVillaNumber", new { id = villaNumber.VillaNo }, _responce);
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
    public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
    {
        try
        {
            if (id == 0)
            {
                _responce.StatucCode = HttpStatusCode.BadRequest;
                _responce.ErrorMessages =
                    new List<string>() { "Id is not exist" };
                return BadRequest(_responce);
            }


            var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);

            if (villaNumber == null)
            {
                _responce.StatucCode = HttpStatusCode.NotFound;
                return NotFound(_responce);
            }

            await _dbVillaNumber.RemoveAsync(villaNumber);

            _responce.StatucCode = HttpStatusCode.NoContent;

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

    [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
    {
        try
        {
            if (updateDTO == null || id != updateDTO.VillaNo)
            {
                _responce.StatucCode = HttpStatusCode.BadRequest;
                return BadRequest(_responce);
            }

            VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);

            await _dbVillaNumber.UpdateAsync(model);

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
}
