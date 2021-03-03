using Sample.KormMultipleDatabases.Domains;
using Sample.KormMultipleDatabases.KORM.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Kros.KORM;

namespace Sample.KormMultipleDatabases.Controllers
{
    [Route("api/{tenant}/[controller]")]
    [ApiController]
    public class WorksController : ControllerBase
    {
        private readonly ITenantDatabaseFactory _databaseFactory;

        public WorksController(ITenantDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        [HttpGet]
        public IEnumerable<Work> GetAll()
        {
            using IDatabase database = _databaseFactory.GetDatabase();

            return database.Query<Work>();
        }
    }
}
