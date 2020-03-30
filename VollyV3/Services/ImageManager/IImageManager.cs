using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Services.ImageManager
{
    public interface IImageManager
    {
        Task<string> UploadOpportunityImageAsync(IFormFile image, string imageName);
    }
}
