using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VollyV3.Models;

namespace VollyV3.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BlogsController(
            ApplicationDbContext context
            )
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Blogs.ToList());
        }
    }
}
