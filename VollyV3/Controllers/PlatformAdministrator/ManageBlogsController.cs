using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VollyV3.Areas.Identity;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.ViewModels.PlatformAdministrator.Blogs;

namespace VollyV3.Controllers.PlatformAdministrator
{
    [Authorize(Roles = nameof(Role.PlatformAdministrator))]
    public class ManageBlogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageBlogsController(
            ApplicationDbContext context
            )
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(new ManageBlogsViewModel()
            {
                Blogs = _context.Blogs.ToList()
            });
        }
        /// <summary>
        /// Add
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAsync(AddBlogViewModel model)
        {
            _context.Add(new Blog()
            {
                Name = model.Name,
                Url = model.Url
            });
            await _context.SaveChangesAsync();
            TempData["Messages"] = $"Blog '{model.Name}' added.";
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(_context.Blogs
                .Where(x => x.Id == id)
                .SingleOrDefault());
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var blog = _context.Blogs
                .Where(x => x.Id == id)
                .SingleOrDefault();

            var blogName = blog.Name;

            _context.Remove(blog);

            await _context.SaveChangesAsync();
            TempData["Messages"] = $"Blog '{blogName}' deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
