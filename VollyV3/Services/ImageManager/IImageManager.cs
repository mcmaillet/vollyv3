using System.IO;
using System.Threading.Tasks;

namespace VollyV3.Services.ImageManager
{
    public interface IImageManager
    {
        Task<string> UploadOpportunityImageAsync(Stream imageStream, string imageName);
    }
}
