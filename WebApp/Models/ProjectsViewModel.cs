using Business.Models;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class ProjectsViewModel
{
    public IEnumerable<ProjectViewModel> Projects { get; set; } = [];

    public AddProjectViewModel AddProjectViewModel { get; set; } = new AddProjectViewModel();
    public EditProjectViewModel EditProjectViewModel { get; set; } = new EditProjectViewModel();

}
