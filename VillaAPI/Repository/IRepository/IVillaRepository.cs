using System;
using System.Linq.Expressions;
using VillaAPI.Models;

namespace VillaAPI.Repository.IRepository;

public interface IVillaRepository : IRepository<Villa>
{
    Task<Villa> UpdateAsync(Villa entity);
}

