using Business.Models;
using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize]
public class ProjectsController(IProjectService projectService, IStatusService statusService, IClientService clientService, IUserService userService) : Controller
{
    private readonly IStatusService _statusService = statusService;
    private readonly IClientService _clientService = clientService;
    private readonly IProjectService _projectService = projectService;
    private readonly IUserService _userService = userService;

    [Route("admin/projects")]
    public async Task<IActionResult> Index()
    {
        var clients = await GetClientsSelectListAsync();
        var statuses = await GetStatusesSelectListAsync();
        IEnumerable<ProjectViewModel> projects = await GetProjectsAsync();
        var users = await GetUsersSelectListAsync();

        var vm = new ProjectsViewModel
        {
            Projects = projects,
            AddProjectViewModel = new AddProjectViewModel() { Clients = clients, Users = users },
            EditProjectViewModel = new EditProjectViewModel() { Clients = clients, Statuses = statuses },
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] AddProjectFormData formData)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value!.Errors.Any())
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return Json(new { success = false, errors });
        }

        var result = await _projectService.CreateProjectAsync(formData);
        if (result != null)
            return LocalRedirect("/admin/projects");

        return Json(new { success = false });
    }


    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var vm = new AddProjectViewModel
        {
            Clients = await GetClientsSelectListAsync(),
        };
        return PartialView("~/Views/Shared/Partials/Project/_AddProjectModal.cshtml", vm);
    }


    private async Task<IEnumerable<SelectListItem>> GetClientsSelectListAsync()
    {
        var result = await _clientService.GetClientsAsync();
        var statusList = result.Result?.Select(s => new SelectListItem
        {
            Value = s.Id,
            Text = s.ClientName,
        });

        return statusList!;
    }

    private async Task<IEnumerable<SelectListItem>> GetUsersSelectListAsync()
    {
        var result = await _userService.GetUsersAsync();
        var statusList = result.Result?.Select(s => new SelectListItem
        {
            Value = s.Id,
            Text = $"{s.FirstName} {s.LastName}",
        });

        return statusList!;
    }


    private async Task<IEnumerable<SelectListItem>> GetStatusesSelectListAsync()
    {
        var result = await _statusService.GetStatusesAsync();
        var statusList = result.Result?.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = s.StatusName,
        });

        return statusList!;
    }

    private async Task<IEnumerable<ProjectViewModel>> GetProjectsAsync()
    {
        IEnumerable<Project> projects = [];
        List<ProjectViewModel> projectViewModels = [];
        try
        {
            var projectResult = await _projectService.GetProjectsAsync();
            if (projectResult.Succeeded && projectResult.Result != null)
            {
                projectViewModels = projectResult.Result.Select(project => new ProjectViewModel
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    Description = project.Description!,
                    ClientName = project.Client?.ClientName ?? "Unknown",
                    Budget =project.Budget,
                     StatusId = project.StatusId,
                }).ToList();
            }

        }
        catch (Exception ex)
        {
            projects = [];
        }

        return projectViewModels;
    }

    [HttpGet]
    [Route("Projects/Edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var projectResult = await _projectService.GetProjectAsync(id);
        if (!projectResult.Succeeded || projectResult.Result == null)
            return NotFound();

        var project = projectResult.Result;
        var users = await GetUsersSelectListAsync(); 
       
        var vm = new EditProjectViewModel
        {
            Id = project.Id,
            ProjectName = project.ProjectName,
            Description = project.Description,
            UserId = project.UserId,
            ClientId =project.ClientId,
            Users = users, 
            Clients = await GetClientsSelectListAsync(),
            Statuses = await GetStatusesSelectListAsync()

            
        };
       
        Console.WriteLine($"Loaded users: {vm.Users.Count()}");
foreach (var user in vm.Users)
{
    Console.WriteLine($"User: {user.Text} ({user.Value})");
}


        return PartialView("~/Views/Shared/Partials/Project/_EditProjectModal.cshtml", vm);
    }



    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _projectService.DeleteProjectAsync(id);
        if (result.Succeeded)
            return LocalRedirect("~/admin/projects");

        else
            Console.WriteLine(result.Error);
            return LocalRedirect("~/admin/projects");


    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/projects")]
    public async Task<IActionResult> Update(EditProjectFormData form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedProject = await _projectService.UpdateProjectAsync(form);

        if (updatedProject != null)
        {
            return RedirectToAction(nameof(Index));
        }

        return BadRequest();
    }



}
