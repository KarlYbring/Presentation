using System.Data;

namespace WebApp.Models;

public class ProjectViewModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Image { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string TimeLeft { get; set; } = null!;
    public decimal? Budget { get; set; }
    public int StatusId { get; set; }
    public string? ClientId { get; set; }

    public IEnumerable<string> Members { get; set; } = [];
}
