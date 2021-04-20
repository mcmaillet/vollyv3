using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VollyV3.Data;
using VollyV3.Models;
using VollyV3.Models.ViewModels.Error;

namespace VollyV3.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        private readonly ApplicationDbContext _context;

        public ErrorController(ILogger<ErrorController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var RequestId = "";
            try
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                RequestId = RequestId?.Trim('|');
                RequestId = RequestId?.Trim('.');

                var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

                _logger.LogError($"RequestId={RequestId} Error={exceptionHandlerPathFeature?.Error} Path={exceptionHandlerPathFeature?.Path}");

                var error = new LoggedError()
                {
                    Id = RequestId,
                    ExceptionType = exceptionHandlerPathFeature?.Error?.GetType().FullName,
                    ExceptionMessage = exceptionHandlerPathFeature?.Error?.Message,
                    ExceptionStackTrace = exceptionHandlerPathFeature?.Error?.StackTrace,
                    Path = exceptionHandlerPathFeature?.Path,
                    CreatedDateTime = DateTime.Now
                };

                _context.LoggedErrors.Add(error);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to log error. e={e}");
            }

            return View(new IndexViewModel()
            {
                RequestId = RequestId
            });
        }
    }
}
