using System.ComponentModel.DataAnnotations;

namespace VollyV3.Areas.Identity
{
    public enum Role
    {
        [Display(Name = "Volunteer")]
        Volunteer,
        [Display(Name = "Organization Administrator")]
        OrganizationAdministrator,
        [Display(Name = "Is configured")]
        IsConfigured,
        [Display(Name = "Platform Administrator")]
        PlatformAdministrator
    }
}
