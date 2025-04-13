using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    
      public class ProjectUpdate
        {
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int StatusId { get; set; }
            public int ClientId { get; set; }
            public int ProjectManagerId { get; set; }
            public int? ProductId { get; set; }

          public string UserId { get; set; } = null!;
    }
    
}
