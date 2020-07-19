using System.ComponentModel.DataAnnotations;

namespace VollyV3.Models.ViewModels.PlatformAdministrator.Blogs
{
    public class AddBlogViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
    }
}
