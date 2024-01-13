using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Saharaviewpoint.Core.Models.Utilities;

/// <summary>
/// Represents a successful result, derived from the <see cref="Result"/> class.
/// </summary>
public class SuccessResult : Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult"/> class with a default success status.
    /// </summary>
    public SuccessResult() : base(true)
    {
        Status = StatusCodes.Status200OK;
        Title = "Operation Successful";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult"/> class with a specified content.
    /// </summary>
    /// <param name="content">The content associated with the successful result.</param>
    public SuccessResult(object content) : base(true)
    {
        Status = StatusCodes.Status200OK;
        Title = "Operation Successful";
        Content = content;
        AddPaging(content);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult"/> class with a specified title.
    /// </summary>
    /// <param name="message">The message associated with the successful result.</param>
    public SuccessResult(string message) : base(true, message)
    {
        Status = StatusCodes.Status200OK;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult"/> class with specified content and message.
    /// </summary>
    /// <param name="content">The content associated with the successful result.</param>
    /// <param name="message">The message associated with the successful result.</param>
    public SuccessResult(string message, object content) : base(true, message)
    {
        Status = StatusCodes.Status200OK;
        Title = "Operation Successful";
        Content = content;
        AddPaging(content);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult"/> class with a specified status code and title.
    /// </summary>
    /// <param name="status">The HTTP status code of the success result.</param>
    /// <param name="message">The additional message associated with the success result.</param>
    public SuccessResult(int status, string message) : base(true, message)
    {
        Status = status;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult"/> class with a specified status code, title, and message.
    /// </summary>
    /// <param name="status">The HTTP status code of the success result.</param>
    /// <param name="content">The content associated with the successful result.</param>
    public SuccessResult(int status, object content) : base(true)
    {
        Status = status;
        Title = "Operation Successful";
        Content = content;
        AddPaging(content);
    }

    // Ignore error related properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new string Detail { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new string Instance { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new string Path { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new TraceInfo TraceInfo { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new IDictionary<string, string[]> ValidationErrors { get; set; }
}

/// <summary>
/// Represents a generic successful result with content, derived from the <see cref="Result"/> class.
/// </summary>
/// <typeparam name="T">The type of the content.</typeparam>
public class SuccessResult<T> : Result<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with a default success status.
    /// </summary>
    public SuccessResult() : base(true)
    {
        Status = StatusCodes.Status200OK;
        Title = "Operation Successful";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with specified content.
    /// </summary>
    /// <param name="content">The content associated with the success result.</param>
    public SuccessResult(T content) : base(true)
    {
        Status = StatusCodes.Status200OK;
        Title = "Operation Successful";
        Content = content;
        AddPaging(content);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with a specified status and content.
    /// </summary>
    /// <param name="status">The HTTP status code of the success result.</param>
    /// <param name="content">The content associated with the success result.</param>
    public SuccessResult(int status, T content) : base(true)
    {
        Status = status;
        Title = "Operation Successful";
        Content = content;
        AddPaging(content);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with a specified title and content.
    /// </summary>
    /// <param name="message">The additional message associated with the success result.</param>
    /// <param name="content">The content associated with the success result.</param>
    public SuccessResult(string message, T content) : base(true, message)
    {
        Status = StatusCodes.Status200OK;
        Content = content;
        AddPaging(content);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with specified status code, title, and content.
    /// </summary>
    /// <param name="status">The HTTP status code of the success result.</param>
    /// <param name="message">The additional message associated with the success result.</param>
    /// <param name="content">The content associated with the success result.</param>
    public SuccessResult(int status, string message, T content) : base(true, message)
    {
        Status = status;
        Content = content;
        AddPaging(content);
    }

    // Ignore error related properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new string Detail { get; set; }

    // Ignore error related properties
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new string Instance { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new string Path { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new TraceInfo TraceInfo { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [JsonIgnore]
    public new IDictionary<string, string[]> ValidationErrors { get; set; }
}