using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Shareviewpoint.Core.Models.Utilities;

/// <summary>
/// Represents an error result, derived from the <see cref="Result"/> class.
/// </summary>
public class ErrorResult : Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResult"/> class with a default failure status.
    /// </summary>
    public ErrorResult() : base(false)
    {
        Status = StatusCodes.Status500InternalServerError;
        Title = "Error Occurred";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResult"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message associated with the result.</param>
    public ErrorResult(string message) : base(false, message)
    {
        Status = StatusCodes.Status500InternalServerError;
        Title = "Error Occurred";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResult"/> class with a specified status code and error message.
    /// </summary>
    /// <param name="status">The HTTP status code of the error result.</param>
    /// <param name="message">The error message associated with the result.</param>
    public ErrorResult(int status, string message) : base(false, message)
    {
        Status = status;
        Title = "Error Occurred";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResult"/> class with a specified title and error message.
    /// </summary>
    /// <param name="title">The title associated with the error result.</param>
    /// <param name="message">The error message associated with the result.</param>
    public ErrorResult(string title, string message) : base(false, message)
    {
        Status = StatusCodes.Status500InternalServerError;
        Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResult"/> class with a specified status code, title, and error message.
    /// </summary>
    /// <param name="status">The HTTP status code of the error result.</param>
    /// <param name="title">The title associated with the error result.</param>
    /// <param name="message">The error message associated with the result.</param>
    public ErrorResult(int status, string title, string message) : base(false, message)
    {
        Status = status;
        Title = title;
    }

    /// <summary>
    /// Additional details about the error.
    /// </summary>
    public string Detail { get; set; }

    /// <summary>
    /// The instance where the error occurred.
    /// </summary>
    public string Instance { get; set; }

    /// <summary>
    /// The path associated with the error.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Information about the trace of the error.
    /// </summary>
#if !DEBUG
    [JsonIgnore]
#endif
    public TraceInfo TraceInfo { get; set; }

    /// <summary>
    /// A collection of validation errors associated with the error.
    /// </summary>
    public ICollection<ValidationError> ValidationErrors { get; set; }
}
