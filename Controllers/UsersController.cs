using Sample.KormMultipleDatabases.Domains;
using Kros.KORM;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Sample.KormMultipleDatabases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatabase _database;

        public UsersController(IDatabase database)
        {
            _database = database;
        }

        [HttpGet]
        public IEnumerable<User> GetAll()
            => _database.Query<User>();
    }
}
