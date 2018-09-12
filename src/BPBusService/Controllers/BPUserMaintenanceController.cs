using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BPBusService.Models;
using Microsoft.AspNetCore.Identity;
using BPBusService.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


/*  Birju Patel
     *  Student ID : 7120967
     *  12/07/2016
     * 
     * BPUserMaintenanceController manages the users according to their roles, by providing neccessary functionality
     * to the required roles of users.
     * The registered users are listed on the Index view with links to Unlock, Delete and Reset their passwords
    */
namespace BPBusService.Controllers
{
    [Authorize(Roles = "administrators")]
    public class BPUserMaintenanceController : Controller
    {
        //public ApplicationDbContext db = new ApplicationDbContext();
        //private UserManager<ApplicationUser> user = new UserManager<ApplicationUser>();

        private readonly UserManager<ApplicationUser> userManager;
        IdentityResult identityResult;


        public BPUserMaintenanceController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        //Lists all the users 
        public IActionResult Index()
        {
            var users = userManager.Users.OrderByDescending(u => u.LockoutEnabled).ThenBy(u => u.UserName).ToList();
            return View(users);
        }

        //Deletes a user and displays any errors if occured
        public async Task<ActionResult> Delete(string userId)
        {
            //identityResult = await userManager.DeleteAsync()
            var user = await userManager.FindByIdAsync(userId);

            if (user != null)
            {
                try
                {
                    //var result = await userManager.DeleteAsync(user);
                    identityResult = await userManager.DeleteAsync(user);


                    if (identityResult.Succeeded)
                    {
                        TempData["message"] = "User Successfully deleted.";
                    }
                    else
                    {
                        TempData["message"] = "Delete failed" + identityResult.Errors.ElementAt(0);
                    }
                }
                catch (Exception ex)
                {
                    TempData["message"] = ex.GetBaseException().Message;
                }
            }
            else
            {
                TempData["message"] = "Please select a user to delete";
            }
            return RedirectToAction("Index");
        }

        //Allows an administrator to reset a user's password and displays any errors if occured
        public async Task<ActionResult> Reset(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (!await userManager.IsInRoleAsync(user, "administrators"))
            {
                //Reset the password
                if (user != null)
                {
                    if (!await userManager.IsInRoleAsync(user, "administrators"))
                    {
                        return View(user);
                    }

                    TempData["message"] = "Administrator Password cannot be changed!.";
                    return RedirectToAction("Index");
                }
                TempData["message"] = "Please select a user to reset the password.";
                return RedirectToAction("Index");
            }

            TempData["message"] = "Only members of the Administrators role can reset the password";
            return RedirectToAction("Index");
        }

        //postback fro confromation of reset password
        public async Task<ActionResult> ResetConfirmed(string userName, string newPassword, string confirmedPassword)
        {
            // Get the user from the userManager

            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                if (newPassword == confirmedPassword)
                {
                    //Reset
                    try
                    {
                        // Remove the user's password
                        await userManager.RemovePasswordAsync(user);
                        identityResult = await userManager.AddPasswordAsync(user, newPassword);
                        if (identityResult.Succeeded)
                        {
                            TempData["message"] = "Password Reset Successfully";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["message"] = "Password Reset Failed" + identityResult.Errors.ElementAt(0);
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["message"] = "Error:" + ex.GetBaseException().Message;
                    }
                    RedirectToAction("Reset", user.Id);
                }
                else
                {
                    TempData["message"] = "Sorry, the passwords do not match";
                    RedirectToAction("Reset", user.Id);
                }
            }
            else
            {
                TempData["message"] = "Please select a user to reset the password";
            }
            return RedirectToAction("Index");
        }

        // Allows an administrator to Lock/Unlock a user and displays any errors if occured
        public async Task<ActionResult> LockUnlock(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user != null)
            {
                if (!await userManager.IsInRoleAsync(user, "administrators"))
                {
                    try
                    {
                        if (user.LockoutEnabled)
                        {
                            user.LockoutEnabled = false;
                            user.LockoutEnd = null;
                            identityResult = await userManager.UpdateAsync(user);
                            TempData["message"] = user.UserName + " has been unlocked.";
                        }
                        else
                        {
                            user.LockoutEnabled = true;
                            user.LockoutEnd = null;
                            identityResult = await userManager.UpdateAsync(user);
                            TempData["message"] = user.UserName + " has been locked.";
                        }
                    }
                    catch(Exception ex)
                    {
                        TempData["message"] = "Error: " + ex.GetBaseException().Message;
                    }
                    return RedirectToAction("Index");
                }
                TempData["message"] = "Administrators cannot be Locked/Unlocked";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Please select a user to Lock/Unlock";
            }

            return RedirectToAction("Index");
        }
    }
}