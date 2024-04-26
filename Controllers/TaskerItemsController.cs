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
            return CreatedAtAction(nameof(GetTaskerItem), new { id = result.Id }, result);
        }

        /*
        - Update Tasker Items
        - Get ID of the user making the request
        - get the existing item out of the database & make sure it belongs to the user
        - update the item with the data in the DTO
        - save the changes
        - send back a success/failure response
        */
        // not returning json so no type parameter needed

        //- get the ID of the tasker item to update
        //- get the updated data from the body of the request
        [HttpPut("{id:guid}")] //all actions take controllers route unless we add to controllers route by the verb 
        [Authorize]
        public async Task<ActionResult> UpdateTaskerItem([FromRoute] Guid id,
                                                            [FromBody] TaskerItemDTO updatedDTO)
        {
            //get the id of the user making the request
            string userId = _userManager.GetUserId(User)!;

            //get the existing item out of the database and make sure it belongs to the user
            TaskerItem? existingItem = await _context.TaskerItems
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id );

            if (existingItem == null)
            {
                return NotFound();
            }
            else
            {
                // update the item with the data in the DTO
                existingItem.Name = updatedDTO.Name;
                existingItem.IsComplete = updatedDTO.IsComplete;

                // save the changes
                _context.TaskerItems.Update(existingItem);
                await _context.SaveChangesAsync();
                //-send back a success/failure response
                return Ok();
            }
        }

        /*
         * -delete Tasker Item
         * - ensure the user is logged in
         * get the id of the item to delete (from the route
         * get the user's id from UserManager
         * get the item from the DB
         * make sure it belongs to the user
         * delete it 
         * send back some response
         */

        [HttpDelete("{id:guid}")]
        [Authorize]

        public async Task<ActionResult> DeleteTaskerItem([FromRoute,] Guid id) //get the ID of the item to delete from route
        {
            string userId = _userManager.GetUserId(User)!; // get the user's ID (from the UserManager)

            //get the item from the DB and make sure it belongs to the user
            TaskerItem? existingItem = await _context.TaskerItems
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);

            if (existingItem == null)
            {
                return NotFound();
            }
            else
            {
                _context.TaskerItems.Remove(existingItem);
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}