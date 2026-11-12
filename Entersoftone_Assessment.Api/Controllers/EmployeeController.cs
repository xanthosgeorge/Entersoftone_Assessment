
using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

/// <summary>
/// Exposes REST endpoints for managing employees.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Retrieves employees with optional search and pagination.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest("Invalid pagination parameters.");

        var employees = await _employeeService.GetAllAsync(
            search, page, pageSize, cancellationToken);

        return Ok(employees);
    }

    /// <summary>
    /// Retrieves an employee by their identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetByIdAsync(
            id, cancellationToken);

        return employee is null ? NotFound() : Ok(employee);
    }

    /// <summary>
    /// Creates a new employee.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeService.CreateAsync(
            request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = employee.Id },
            employee);
    }

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _employeeService.UpdateAsync(
            id, request, cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes an employee.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _employeeService.DeleteAsync(
            id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
