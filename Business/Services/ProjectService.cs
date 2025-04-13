using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;

namespace Business.Services;

public interface IProjectService
{
    Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync();
    Task<ProjectResult<Project>> GetProjectAsync(string Id);
    Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData);
    Task<ProjectResult> UpdateProjectAsync(EditProjectFormData formData);
    Task<ProjectResult> DeleteProjectAsync(string id);
}

public class ProjectService(IProjectRepository projectRepository, IStatusService statusService) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusService = statusService;

    public async Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData)
    {
        if (formData == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Invalid form data." };

        var statusResult = await _statusService.GetStatusByIdAsync(1);
        var status = statusResult.Result;


        var projectEntity = new ProjectEntity
        {
            Id = Guid.NewGuid().ToString(),
            ProjectName = formData.ProjectName,
            Description = formData.Description,
            StartDate = formData.StartDate,
            EndDate = formData.EndDate,
            Budget = formData.Budget,
            ClientId = formData.ClientId,
            UserId = formData.UserId,
            StatusId = status!.Id,
            Created = DateTime.Now
        };

        projectEntity.StatusId = status!.Id;

        var result = await _projectRepository.AddAsync(projectEntity);

        return result.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 201 }
            : new ProjectResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    public async Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync()
    {
        var response = await _projectRepository.GetAllAsync
            (
                orderByDescending: true,
                sortBy: x => x.Created,
                where: null,
                take: 0,
                include => include.User,
                include => include.Client,
                include => include.Status
            );

        return new ProjectResult<IEnumerable<Project>> { Succeeded = true, StatusCode = 200, Result = response.Result };
    }

    public async Task<ProjectResult<Project>> GetProjectAsync(string Id)
    {
        var response = await _projectRepository.GetAsync
            (
                x => x.Id.ToLower() == Id.Trim().ToLower(),
                include => include.Client,
                include => include.Status,
                include => include.User
            );

        return response.Succeeded
            ? new ProjectResult<Project> { Succeeded = true, StatusCode = 200, Result = response.Result }
            : new ProjectResult<Project> { Succeeded = false, StatusCode = 404, Error = $"Project '{Id}' not found." };
    }



    //Fått hjälp av CHATgpt för att strukturera upp denna metod
    public async Task<ProjectResult> UpdateProjectAsync(EditProjectFormData formData)
{
    if (formData == null)
    {
        return new ProjectResult
        {
            Succeeded = false,
            StatusCode = 400,
            Error = "Not all required fields are supplied."
        };
    }

    var existingProjectResponse = await _projectRepository.GetEntityAsync(x => x.Id == formData.Id);

    if (!existingProjectResponse.Succeeded || existingProjectResponse.Result == null)
    {
        return new ProjectResult
        {
            Succeeded = false,
            StatusCode = 404,
            Error = $"Project '{formData.Id}' was not found."
        };
    }

    var existingProject = existingProjectResponse.Result;

    existingProject.ProjectName = formData.ProjectName;
    existingProject.Description = formData.Description;
    existingProject.StartDate = formData.StartDate;
    existingProject.EndDate = formData.EndDate;
    existingProject.Budget = formData.Budget;
    existingProject.ClientId = formData.ClientId;
    existingProject.StatusId = formData.StatusId;
    existingProject.UserId = formData.UserId;

    var updateResult = await _projectRepository.UpdateAsync(existingProject);

    return updateResult.Succeeded
        ? new ProjectResult { Succeeded = true, StatusCode = 200 }
        : new ProjectResult { Succeeded = false, StatusCode = updateResult.StatusCode, Error = updateResult.Error };
}



    public async Task<ProjectResult> DeleteProjectAsync(string id)
    {
        var projectEntityResponse = await _projectRepository.GetEntityAsync(x => x.Id == id);

        if (!projectEntityResponse.Succeeded || projectEntityResponse.Result == null)
        {
            return new ProjectResult
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "Project Not Found"
            };
        }

        var deleteResult = await _projectRepository.DeleteAsync(projectEntityResponse.Result);

        return deleteResult.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 200 }
            : new ProjectResult { Succeeded = false, StatusCode = deleteResult.StatusCode, Error = deleteResult.Error };
    }
}

