using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.Contracts;
using Users.Exceptions;
using Users.Filters;
using Users.Repository.Contexts;
using Users.Repository.Entities;

namespace Users.Controllers
{
    [ApiController]
    [Route("api/users")]
    [TypeFilter(typeof(ApiExceptionFilter))]
    public class UsersController : ControllerBase
    {
        private readonly UsersContext _repo;

        public UsersController(UsersContext repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody, Required] UserContract userContract,
            CancellationToken cancellationToken)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(
                u => u.Username.Equals(userContract.Username),
                cancellationToken);

            if (user != null)
                throw new UsernameNotUniqueException(userContract.Username);

            var userAdded = (await _repo.Users.AddAsync(new User {Username = userContract.Username}, cancellationToken))
                .Entity;
            await _repo.SaveChangesAsync(cancellationToken);

            return Ok(userAdded);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(CancellationToken cancellationToken)
        {
            var users = await _repo.Users.OrderBy(u => u.Id).ToListAsync(cancellationToken);
            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser([Required] int id, [FromBody, Required] UserContract userContract,
            CancellationToken cancellationToken)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null)
                throw new UserNotFoundException(id);
            
            var userCheck = await _repo.Users.FirstOrDefaultAsync(
                u => u.Username.Equals(userContract.Username) &&
                     id != u.Id, cancellationToken);

            if (userCheck != null)
                throw new UsernameNotUniqueException(userContract.Username);

            user.Username = userContract.Username;
            await _repo.SaveChangesAsync(cancellationToken);

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveUser([Required] int id, CancellationToken cancellationToken)
        {
            var user = await _repo.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null)
                throw new UserNotFoundException(id);

            _repo.Remove(user);

            await _repo.SaveChangesAsync(cancellationToken);

            return Ok(user);
        }
    }
}