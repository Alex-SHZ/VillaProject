using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Controllers.v1;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Models.DTO;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers.v2;

[Route("api/v{version:apiVersion}/VillaNumberAPI")]
[ApiController]
[ApiVersion("2.0")]
public class VillaNumberAPIv2Controller : ControllerBase
{
    private readonly IVillaNumberRepository _dbVillaNumber;
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;
    private readonly ILogger<VillaNumberAPIv2Controller> _logger;
    protected APIResponse _responce;

    public VillaNumberAPIv2Controller(
        ILogger<VillaNumberAPIv2Controller> logger,
        IVillaNumberRepository dbVillaNumber,
        IVillaRepository dbVilla,
        IMapper mapper)
    {
        _logger = logger;
        _dbVilla = dbVilla;
        _dbVillaNumber = dbVillaNumber;
        _mapper = mapper;
        _responce = new();
    }

    [HttpGet()]
    public IEnumerable<string> Get()
    {
        return new string[] { "Alex", "SHZ" };
    }
}

