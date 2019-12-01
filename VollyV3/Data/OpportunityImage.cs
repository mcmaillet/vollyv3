using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Data
{
    public class OpportunityImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public int OpportunityId { get; set; }
        public virtual Opportunity Opportunity { get; set; }
    }
}
