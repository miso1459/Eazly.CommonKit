using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;

namespace Eazly.CommonKit.Module.TemplateURL.Repository
{
    public interface ITemplateURLRepository
    {
        IEnumerable<Models.TemplateURL> GetTemplateURLs(int ModuleId);
        Models.TemplateURL GetTemplateURL(int TemplateURLId);
        Models.TemplateURL GetTemplateURL(int TemplateURLId, bool tracking);
        Models.TemplateURL AddTemplateURL(Models.TemplateURL TemplateURL);
        Models.TemplateURL UpdateTemplateURL(Models.TemplateURL TemplateURL);
        void DeleteTemplateURL(int TemplateURLId);
    }

    public class TemplateURLRepository : ITemplateURLRepository, ITransientService
    {
        private readonly IDbContextFactory<TemplateURLContext> _factory;

        public TemplateURLRepository(IDbContextFactory<TemplateURLContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<Models.TemplateURL> GetTemplateURLs(int ModuleId)
        {
            using var db = _factory.CreateDbContext();
            return db.TemplateURL.Where(item => item.ModuleId == ModuleId).ToList();
        }

        public Models.TemplateURL GetTemplateURL(int TemplateURLId)
        {
            return GetTemplateURL(TemplateURLId, true);
        }

        public Models.TemplateURL GetTemplateURL(int TemplateURLId, bool tracking)
        {
            using var db = _factory.CreateDbContext();
            if (tracking)
            {
                return db.TemplateURL.Find(TemplateURLId);
            }
            else
            {
                return db.TemplateURL.AsNoTracking().FirstOrDefault(item => item.TemplateURLId == TemplateURLId);
            }
        }

        public Models.TemplateURL AddTemplateURL(Models.TemplateURL TemplateURL)
        {
            using var db = _factory.CreateDbContext();
            db.TemplateURL.Add(TemplateURL);
            db.SaveChanges();
            return TemplateURL;
        }

        public Models.TemplateURL UpdateTemplateURL(Models.TemplateURL TemplateURL)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(TemplateURL).State = EntityState.Modified;
            db.SaveChanges();
            return TemplateURL;
        }

        public void DeleteTemplateURL(int TemplateURLId)
        {
            using var db = _factory.CreateDbContext();
            Models.TemplateURL TemplateURL = db.TemplateURL.Find(TemplateURLId);
            db.TemplateURL.Remove(TemplateURL);
            db.SaveChanges();
        }
    }
}
