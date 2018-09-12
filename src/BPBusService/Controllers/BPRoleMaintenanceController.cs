using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BPBusService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;


/*  Birju Patel
     *  Student ID : 7120967
     *  12/07/2016
     * 
     * BPRoleMaintenanceController allows the administrators to maintain the roles by creating new roles and adding/removing
     * users in various roles
     * 
    */
namespace BPBusService.Controllers
{
    [Authorize(Roles = "administrators")]
    public class BPRoleMaintenanceController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        IdentityResult identityResult;

        public BPRoleMaintenanceController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            var roles = roleManager.Roles.OrderBy(n => n.Name);
            return View(roles);
        }

        // Creates a new role and displays any errors if occured
        public async Task<ActionResult> Create(string roleToAdd)
        {
            if (roleToAdd != null && roleToAdd.Trim() != "")
            {
                var role = await roleManager.FindByNameAsync(roleToAdd);
                if (role == null)
                {
                    try
                    {
                        roleToAdd = roleToAdd.Trim();
                        identityResult = await roleManager.CreateAsync(new IdentityRole(roleToAdd));
                        if(identityResult.Succeeded)
                        {
                            TempData["message"] = "Role Successfully added";
                        }
                        else
                        {
                            TempData["message"] = "Creating Role Failed: " + identityResult.Errors.ElementAt(0).Description;
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["message"] = "Error: " + ex.GetBaseException().Message;
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Sorry, Role already exists";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["message"] = "Please enter a Role to create";
            }
            return RedirectToAction("Index");
        }

        //Deletes a role and displays any errors if occured
        public async Task<ActionResult> Delete(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role != null)
            {
                if (role.Name != "administrators")
                {
                    if(role.Users.Count == 0)
                    {
                        try
                        {
                            identityResult = await roleManager.DeleteAsync(role);
                            if(identityResult.Succeeded)
                            {
                                TempData["message"] = role.Name+" succesfully Deleted:";
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["message"] = "Deleting Role failed:  " + identityResult.Errors.ElementAt(0).Description;
                            }
                        }
                        catch(Exception ex)
                        {
                            TempData["message"] = "Error: " + ex.GetBaseException().Message;
                        }
                    }
                    else
                    {
                        // The role to be deleted has some users in it
                        var users = await userManager.GetUsersInRoleAsync(role.Name);
                        Response.Cookies.Append("roleToBeDeleted", role.Id.ToString());
                        ViewBag.role = users;
                        return View("Delete", users);
                    }
                }
                else
                {
                    TempData["message"] = "Administrators Role cannot be Deleted";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["message"] = "Please select a Role to delete";
            }
            return RedirectToAction("Index");
        }

        // Deletes a role which has users in it after confirmation
        public async Task<ActionResult> DeleteConfirmed(string roleId)
        {
            var role = await roleManager.FindByIdAsync(Request.Cookies["roleToBeDeleted"].ToString());

            if (role != null)
            {
                try
                {
                    identityResult = await roleManager.DeleteAsync(role);
                    if (identityResult.Succeeded)
                    {
                        TempData["message"] = role.Name+ " successfully deleted.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Delete role failed: " + identityResult.Errors.ElementAt(0).Description;
                    }
                }
                catch (Exception error)
                {
                    TempData["message"] = "Error: " + error.GetBaseException().Message;
                }
                return RedirectToAction("Delete", new { roleId = role.Id });
            }
            TempData["message"] = "Please select a role to delete.";
            return RedirectToAction("Index");
        }

        //Finds all the members of the selected role and displays them
        public async Task<ActionResult> Manage(string roleId)
        {
            
            var role = await roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
                var usersNotInRole = userManager.Users.ToList().Except(usersInRole);

                ViewBag.UserId = new SelectList(usersNotInRole, "Id", "UserName");
                Response.Cookies.Append("roleId", roleId.ToString());
                Response.Cookies.Append("roleName", role.Name.ToString());
                
                return View(usersInRole);
            }
            else
            {
                TempData["message"] = "Please select a Role to manage";
            }
            return RedirectToAction("Index");
        }

        //Adds a user to a specific role and displays any errors if occured
        public async Task<ActionResult> Add(string userId)
        {
            if (Request.Cookies["roleId"] != null)
            {
                var user = await userManager.FindByIdAsync(userId);
                if(user != null)
                {
                    var role = await roleManager.FindByIdAsync(Request.Cookies["roleId"].ToString());
                    try
                    {
                        identityResult = await userManager.AddToRoleAsync(user, role.Name);
                        if(identityResult.Succeeded)
                        {
                            TempData["message"] = "User successfully added to Role - " + role.Name;
                        }
                        else
                        {
                            TempData["message"] = "Adding User failed: " + identityResult.Errors.ElementAt(0).Description;
                        }
                    }
                    catch(Exception ex)
                    {
                        TempData["message"] = "Error: " + ex.GetBaseException().Message;
                    }
                    return RedirectToAction("Manage", new { roleId = role.Id});
                }
                TempData["message"] = "Please select a Role to add the user";
                return RedirectToAction("Manage", new { roleId = Request.Cookies["roleId"]});
            }
            else
            {
                TempData["message"] = "Please select a Role to add the user";
            }
            return RedirectToAction("Index");
        }

        //Removes a user from a role and displays any errors if occured
        public async Task<ActionResult> Remove(string userId)
        {

            if (Request.Cookies["roleId"].ToString() != null)
            {
                var role = await roleManager.FindByIdAsync(Request.Cookies["roleId"].ToString());
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {   
                    try
                    {
                        identityResult = await userManager.RemoveFromRoleAsync(user, role.Name);
                        if (identityResult.Succeeded)
                        {
                            TempData["message"] = user.UserName + " successfully removed from " + role.Name;
                        }
                        else
                        {
                            TempData["message"] = "Removing " + user.UserName + "failed: " + identityResult.Errors.ElementAt(0).Description;
                        }
                    }
                    catch(Exception ex)
                    {
                        TempData["message"] = "Error: " + ex.GetBaseException().Message;
                    }
                    return RedirectToAction("Manage", new { roleId = role.Id });
                }
                TempData["message"] = "Please select a user to remove";
                return RedirectToAction("Manage", new { roleId = role.Id });
            }
            else
            {
                TempData["message"] = "Please select a Role to remove the user";
            }

            return RedirectToAction("Index");
        }
    }
}