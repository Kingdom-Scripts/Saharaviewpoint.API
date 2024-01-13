using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Saharaviewpoint.Core.Models.Utilities;

/// <summary>
/// Represents an error result, derived from the <see cref="Result"/> class.
/// </summary>
public class BadErrorResult : Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadErrorResult"/> class with a default failure status.
    /// </summary>
    public BadErrorResult() : base(false)
    {
        Status = StatusCodes.Status400BadRequest;
        Title = "Invalid Request";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadErrorResult"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message associated with the result.</param>
    public BadErrorResult(string message) : base(false, message)
    {
        Status = StatusCodes.Status400BadRequest;
        Title = "Invalid Request";
    }

    /// <summary>
    /// Additional details about the error.
    /// </summary>
    public new string Detail { get; set; }

    /// <summary>
    /// The instance where the error occurred.
    /// </summary>
    public new string Instance { get; set; }

    /// <summary>
    /// The path associated with the error.
    /// </summary>
    public new string Path { get; set; }

    // Ignore Success related properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new object Content { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new Paging Paging { get; set; }
}

/// <summary>
/// Represents an error result.
/// </summary>
public class BadErrorResult<T> : Result<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadErrorResult"/> class with a default failure status.
    /// </summary>
    public BadErrorResult() : base(false)
    {
        Status = StatusCodes.Status400BadRequest;
        Title = "Invalid Request";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadErrorResult"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message associated with the result.</param>
    public BadErrorResult(string message) : base(false, message)
    {
        Status = StatusCodes.Status400BadRequest;
        Title = "Invalid Request";
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

    // Ignore Success related properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new T Content { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new Paging Paging { get; set; }
}