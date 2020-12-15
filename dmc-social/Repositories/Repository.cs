using System;
using System.Linq;
using System.Threading.Tasks;
using DmcSocial.Interfaces;
using DmcSocial.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DmcSocial.Repositories
{
  public class Repository : IRepository
  {
    private readonly AppDbContext _db;
    public Repository(AppDbContext db)
    {
      _db = db;
    }
    public void Add<T>(T entity, string actor) where T : BaseEntity
    {
      var now = DateTimeOffset.Now;
      entity.DateCreated = now;
      entity.LastModifiedTime = now;
      entity.CreatedBy = actor;
      entity.LastModifiedBy = actor;
      _db.Add(entity);
    }

    public void Delete<T>(T entity, string actor) where T : BaseEntity
    {
      var now = DateTimeOffset.Now;
      entity.DateRemoved = now;
      entity.RemovedBy = actor;
      _db.Attach(entity).Property(u => new { u.DateRemoved, u.RemovedBy }).IsModified = true;
    }

    public IQueryable<T> GetQuery<T>() where T : BaseEntity
    {
      return _db.Set<T>().Where(u => u.DateRemoved == null);
    }

    public void Update<T>(T entity, string actor) where T : BaseEntity
    {
      var now = DateTimeOffset.Now;
      entity.LastModifiedTime = now;
      entity.LastModifiedBy = actor;
      _db.Attach(entity).Property(u => new { u.LastModifiedTime, u.LastModifiedBy }).IsModified = true;
    }
  }
}