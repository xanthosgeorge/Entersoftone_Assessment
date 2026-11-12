
using EmployeeManagement.Application.DTOs.Projects;
using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

/// <summary>
/// Provides REST API endpoints for managing projects.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<ProjectDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var projects = await _projectService.GetAllAsync(cancellationToken);

        return Ok(projects);
    }

    /// <summary>
    /// Retrieves a project by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var project = await _projectService.GetByIdAsync(
            id, cancellationToken);

        return project is null ? NotFound() : Ok(project);
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDto>> Create(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var project = await _projectService.CreateAsync(
            request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = project.Id },
            project);
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _projectService.UpdateAsync(
            id, request, cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a project and its employee assignments,
    /// without deleting the associated employees.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _projectService.DeleteAsync(
            id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
