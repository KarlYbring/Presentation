﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class AddProjectViewModel
{
    [DataType(DataType.Upload)]
    [Display(Name = "Project Image", Prompt = "Select project image")]
    public IFormFile? Image { get; set; }

    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Project Name", Prompt = "Enter project name")]
    public string ProjectName { get; set; } = null!;

    [DataType(DataType.Text)]
    [Display(Name = "Description", Prompt = "Type something")]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Start Date", Prompt = "Enter start date")]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "End Date", Prompt = "Enter end date")]
    public DateTime? EndDate { get; set; }

    [DataType(DataType.Currency)]
    [Display(Name = "Budget", Prompt = "Enter budget")]
    public decimal? Budget { get; set; }

    [Required]
    [Display(Name = "Client", Prompt = "Select client")]
    public string ClientId { get; set; } = null!;

    [Display(Name = "Members", Prompt = "Select a member")]
    public string UserId { get; set; } = null!;



    public IEnumerable<SelectListItem> Clients { get; set; } = [];
    public IEnumerable<SelectListItem> Users { get; set; } = [];

}
