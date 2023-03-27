using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Repository.IRepository;
namespace VillaAPI.Repository
{
	public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        private readonly ApplicationDbContext _db;

        public VillaRepository(ApplicationDbContext db) : base(db)
		{
            _db = db;
		}

        public async Task<Villa> UpdateAsync(Villa entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Villas.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}

