namespace EmployeeManagement.Application.Exceptions
{
    /// <summary>
    /// Represents a conflict with an existing application resource.
    /// </summary>
    public sealed class ConflictException(string message) : Exception(message);
}
