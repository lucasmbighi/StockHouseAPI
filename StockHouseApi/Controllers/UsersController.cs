using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockHouseApi.Models;

namespace StockHouseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UsersController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("/users")]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .Include(u => u.StockItems)
                .Select(u => UserToResponseDTO(u))
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet]
        [Route("/users/{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetUserById(Guid id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            var userById = await _context.Users
                .Include(u => u.StockItems)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (userById == null)
            {
                return NotFound();
            }

            return Ok(UserToResponseDTO(userById));
        }

        [HttpGet]
        [Route("/users/{id}/stockItems")]
        public async Task<ActionResult<IEnumerable<GroceryItemDTO>>> GetItemsOfUserWithId(Guid id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            var userById = await _context.Users
                .Include(u => u.StockItems)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (userById == null)
            {
                return NotFound();
            }

            var stockItems = await _context.StockItems
                .Include(i => i.User)
                .Where(i => i.UserId == id)
                .Select(i => ItemToDTO(i))
                .ToListAsync();

            if (stockItems == null)
            {
                return NotFound();
            }

            return stockItems;
        }

        [HttpGet]
        [Route("/users/{id}/stockItems/{itemId}")]
        public async Task<ActionResult<GroceryItemDTO>> GetItemWithIdOfUserWithId(Guid id, Guid itemId)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            var userById = await _context.Users
                .Include(u => u.StockItems)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (userById == null)
            {
                return NotFound();
            }

            //var itemById = _context.StockItems
            //    //.Include(i => i.User)
            //    //.Where(i => i.UserId == id)
            //    .Select(i => ItemToDTO(i))
            //    .FirstOrDefaultAsync(i => i.Id == itemId);

            var itemById = await _context.StockItems
                .Include(i => i.User)
                .Where(i => i.UserId == id)
                //.Select(i => ItemToDTO(i))
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (itemById == null)
            {
                return NotFound();
            }

            return Ok(ItemToDTO(itemById));
        }

        [HttpPut]
        [Route("/users/{id}/updatePassword")]
        public async Task<IActionResult> UpdatePassword(Guid id, ChangePasswordRequestDTO changePasswordPayload)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            var userById = await _context.Users
                .Include(u => u.StockItems)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (userById == null)
            {
                return NotFound();
            }

            userById.Password = changePasswordPayload.Password;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut]
        [Route("/users/{id}/stockItems/{itemId}")]
        public async Task<ActionResult<UserResponseDTO>> AddItemToUserWithId(Guid id, Guid itemId, GroceryItemDTO itemPayload)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            var userById = await _context.Users
                .Include(u => u.StockItems)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (userById == null)
            {
                return NotFound();
            }

            var itemById = await _context.StockItems
                .Where(i => i.UserId == id)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (itemById == null)
            {
                return NotFound();
            }

            itemById = _mapper.Map<GroceryItem>(itemPayload);

            _context.StockItems.Update(itemById);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Route("/users")]
        public async Task<ActionResult<UserResponseDTO>> CreateUser(CreateUserRequestDTO userPayload)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Context.Users' is null.");
            }

            var newUser = _mapper.Map<User>(userPayload);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = newUser.Id },
                UserToResponseDTO(newUser));
        }

        [HttpPost]
        [Route("/users/{id}/stockItems")]
        public async Task<ActionResult<GroceryItemDTO>> AddItemToUserWithId(Guid id, GroceryItemDTO itemPayload)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Context.Users' is null.");
            }

            var userById = await _context.Users
                .Include(u => u.StockItems)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (userById == null)
            {
                return NotFound();
            }

            var newItem = _mapper.Map<GroceryItem>(itemPayload);

            newItem.User = userById;
            newItem.UserId = id;

            _context.StockItems.Add(newItem);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetItemWithIdOfUserWithId),
                new { id = newItem.Id },
                ItemToDTO(newItem));
        }

        [HttpDelete]
        [Route("/users/{id}/stockItems/{itemId}")]
        public async Task<IActionResult> DeleteItemWithIdOfUserWithId(Guid id, Guid itemId)
        {
            if (_context.StockItems == null)
            {
                return NotFound();
            }

            var item = await _context.StockItems.FindAsync(itemId);

            if (item == null)
            {
                return NotFound();
            }

            _context.StockItems.Remove(item);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static ChangePasswordRequestDTO UserToChangePasswordRequestDTO(User user) =>
            new ChangePasswordRequestDTO
            {
                Id = user.Id,
                Password = user.Password
            };

        private static UserResponseDTO UserToResponseDTO(User user) =>
            new UserResponseDTO
            {
                Id = user.Id,
                Username = user.Username
            };

        private static GroceryItemDTO ItemToDTO(GroceryItem stockItem) =>
            new GroceryItemDTO
            {
                Id = stockItem.Id,
                Name = stockItem.Name,
                Quantity = stockItem.Quantity,
                Unity = stockItem.Unity,
                Description = stockItem.Description,
                UserId = stockItem.UserId
            };
    }
}
