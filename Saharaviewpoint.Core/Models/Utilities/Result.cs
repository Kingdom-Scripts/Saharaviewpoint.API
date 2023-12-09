namespace Saharaviewpoint.Core.Models.Utilities;

/// <summary>
/// Represents the outcome of an operation.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool Success { get; set; } = false;

    /// <summary>
    /// The title associated with the result.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Represents the HTTP status code of the result.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Additional message providing context or details about the result.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    public Result() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class with a specified success status.
    /// </summary>
    /// <param name="success">A value indicating whether the operation was successful.</param>
    public Result(bool success)
    {
        Success = success;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class with a specified success status and message.
    /// </summary>
    /// <param name="success">A value indicating whether the operation was successful.</param>
    /// <param name="message">The additional message associated with the result.</param>
    public Result(bool success, string message) : this(success)
    {
        Message = message;
    }
}
