namespace EmployeeManagement.Api.Exceptions
{
    /// <summary>
    /// Represents a reference to a resource that does not exist.
    /// </summary>
    public sealed class ResourceNotFoundException(string message)
        : Exception(message);
}
