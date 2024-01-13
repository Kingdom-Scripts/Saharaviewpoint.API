using Saharaviewpoint.Core.Interfaces;
using System.Text.Json.Serialization;

namespace Saharaviewpoint.Core.Models.Utilities;

/// <summary>
/// Represents the outcome of an operation.
/// </summary>
/// <typeparam name="T">The type of the content.</typeparam>
public class Result<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    public Result()
    { }

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

    /// <summary>
    /// Additional details about the error.
    /// </summary>
    public string Detail { get; set; }

    /// <summary>
    /// The instance where the error occurred.
    /// </summary>
    public string Instance { get; set; }

    /// <summary>
    /// Additional message providing context or details about the result.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// The path associated with the error.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Represents the HTTP status code of the result.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool Success { get; set; } = false;

    /// <summary>
    /// The title associated with the result.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// A collection of validation errors associated with the error.
    /// </summary>
    public IDictionary<string, string[]> ValidationErrors { get; set; }

    /// <summary>
    /// Information about the trace of the error.
    /// </summary>
#if !DEBUG
    [JsonIgnore]
#endif
    public TraceInfo TraceInfo { get; set; }

    /// <summary>
    /// The content associated with the success result.
    /// </summary>
    public T Content { get; set; }

    /// <summary>
    /// The paging information associated with the success result.
    /// </summary>
    public Paging Paging { get; set; }

    /// <summary>
    /// Adds paging information based on the provided content.
    /// </summary>
    /// <param name="content">The content with paging information.</param>
    public void AddPaging(T content)
    {
        if (content is IPagedList x)
        {
            Paging = new Paging
            {
                PageIndex = x.PageIndex,
                PageSize = x.PageSize,
                TotalPages = x.TotalPages,
                TotalItems = x.TotalItems,
                HasNextPage = x.HasNextPage,
                HasPreviousPage = x.HasPreviousPage,
            };
        }
    }
}

/// <summary>
/// Represents the outcome of an operation.
/// </summary>
public class Result : Result<object>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class with a specified success status.
    /// </summary>
    /// <param name="success">A value indicating whether the operation was successful.</param>
    public Result(bool success) : base(success)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class with a specified success status and message.
    /// </summary>
    /// <param name="success">A value indicating whether the operation was successful.</param>
    /// <param name="message">The additional message associated with the result.</param>
    public Result(bool success, string message) : base(success, message)
    {
    }
}
