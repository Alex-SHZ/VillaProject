using System;
using System.Collections.Generic;
using System.Linq;
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

[Route("api/VillaAPI")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;
    private readonly ILogger<VillaAPIController> _logger;

    public VillaAPIController(
        ILogger<VillaAPIController> logger,
        IVillaRepository dbVilla,
        IMapper mapper)
    {
        _logger = logger;
        _dbVilla = dbVilla;
        _mapper = mapper;
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
    {
        IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
        return Ok(_mapper.Map<List<VillaDTO>>(villaList));
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(200, Type =typeof(VillaDTO))]
    public async Task<ActionResult<VillaDTO>> GetVilla(int id)
    {
        if (id == 0)
            return BadRequest();

        var villa = await _dbVilla.GetAsync(u => u.Id == id);

        if (villa == null)
            return NotFound();

        return Ok(_mapper.Map<VillaDTO>(villa));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
    {
        if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
        {
            ModelState.AddModelError("CustomError", "Villa already Exists!");
            return BadRequest(ModelState);
        }

        if (createDTO == null)
            return BadRequest(createDTO);


        Villa model = _mapper.Map<Villa>(createDTO);

        await _dbVilla.CreateAsync(model);


        return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    public async Task<IActionResult> DeleteVilla(int id)
    {
        if (id == 0)
            return BadRequest();


        var villa = await _dbVilla.GetAsync(u => u.Id == id);

        if (villa == null)
            return NotFound();

        await _dbVilla.RemoveAsync(villa);

        return NoContent();

    }

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
    {
        if (updateDTO == null || id != updateDTO.Id)
            return BadRequest();

        Villa model = _mapper.Map<Villa>(updateDTO);

        await _dbVilla.UpdateAsync(model);

        return NoContent();
    }

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
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

