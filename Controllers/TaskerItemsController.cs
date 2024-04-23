using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskerBCAPI.Data;
using TaskerBCAPI.Models;

namespace TaskerBCAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskerItemsController : ControllerBase
    {

        private readonly ApplicationDbContext _context; //readonly assigns to a varaible once
        private readonly UserManager<IdentityUser> _userManager;

        public TaskerItemsController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context; //constructor
            _userManager = userManager;
        }

        [HttpGet, Authorize] //GET: api/TaskerItems
        public async Task<ActionResult<List<TaskerItemDTO>>> GetTaskerItems()
        {

            string userId = _userManager.GetUserId(User);
            List<TaskerItem> taskerItems = await _context.TaskerItems.Where(t => t.UserId == userId).ToListAsync();
            //Database Query, taskerItems variable of type List<TaskerItem> is
            //now equal to the list from the database 

            List<TaskerItemDTO> results = new List<TaskerItemDTO>();

            foreach (TaskerItem item in taskerItems)
            {
                TaskerItemDTO itemDTO = new TaskerItemDTO()
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsComplete = item.IsComplete,
                };
                results.Add(itemDTO);
            }
            return Ok(results);
        }

        [HttpGet("{id:guid}"), Authorize] //Get: api/TaskerItems/your-items-guid
        public async Task<ActionResult<TaskerItemDTO>> GetTaskerItem([FromRoute] Guid id)
        {
            string userId = _userManager.GetUserId(User)!;

            TaskerItem? taskerItem = await _context.TaskerItems.FirstOrDefaultAsync(ti => ti.Id == id && ti.UserId == userId);

            if (taskerItem == null)
            {
                return NotFound();
            }
            else
            {
                TaskerItemDTO result = new TaskerItemDTO();
                result.Id = taskerItem.Id;
                result.Name = taskerItem.Name;
                result.IsComplete = taskerItem.IsComplete;

                return Ok(result);
            }

        }

        [HttpPost] //POST: api/TaskerItems
        [Authorize]

        public async Task<ActionResult<TaskerItemDTO>> CreateTaskerItem([FromBody] TaskerItemDTO newTaskerItem)
        {
            string userId = _userManager.GetUserId(User)!;
            //gets ID through GetUserID Method
            //take the DTO abnd turn it into a real tasker item
            //ensure that the new TaskerItem belongs to the user who submits it
            TaskerItem dbTaskerItem = new TaskerItem()
            {
                Name = newTaskerItem.Name,
                IsComplete = newTaskerItem.IsComplete,
                UserId = userId,
                //ensure that the new TaskerItem belongs to the user who submits it
            };
            //save it to the database

            _context.TaskerItems.Add(dbTaskerItem);
            await _context.SaveChangesAsync();
            //return success status code + created tasker item as a DTO

            TaskerItemDTO result = new TaskerItemDTO()
            {
                Id = dbTaskerItem.Id,
                Name = dbTaskerItem.Name,
                IsComplete = dbTaskerItem.IsComplete,
            };
            return CreatedAtAction(nameof(GetTaskerItem), new {id = result.Id}, result);
        }

    }
}