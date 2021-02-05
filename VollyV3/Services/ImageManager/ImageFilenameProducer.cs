using System;

namespace VollyV3.Services.ImageManager
{
    public class ImageFilenameProducer
    {
        public static string Create()
        {
            return $"{DateTime.Now.Ticks}_{Guid.NewGuid()}";
        }
    }
}
