using Microsoft.AspNetCore.Http;
using Shareviewpoint.Core.Interfaces;

namespace Shareviewpoint.Core.Models.Utilities;

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
    /// Initializes a new instance of the <see cref="SuccessResult"/> class with a specified title.
    /// </summary>
    /// <param name="title">The title associated with the success result.</param>
    public SuccessResult(string title) : base(true)
    {
        Status = StatusCodes.Status200OK;
        Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult"/> class with a specified status code and title.
    /// </summary>
    /// <param name="status">The HTTP status code of the success result.</param>
    /// <param name="title">The title associated with the success result.</param>
    public SuccessResult(int status, string title) : base(true)
    {
        Status = status;
        Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult"/> class with a specified title and message.
    /// </summary>
    /// <param name="title">The title associated with the success result.</param>
    /// <param name="message">The additional message associated with the success result.</param>
    public SuccessResult(string title, string message) : base(true, message)
    {
        Status = StatusCodes.Status200OK;
        Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult"/> class with a specified status code, title, and message.
    /// </summary>
    /// <param name="status">The HTTP status code of the success result.</param>
    /// <param name="title">The title associated with the success result.</param>
    /// <param name="message">The additional message associated with the success result.</param>
    public SuccessResult(int status, string title, string message) : base(true, message)
    {
        Status = status;
        Title = title;
    }
}

/// <summary>
/// Represents a generic successful result with content, derived from the <see cref="Result"/> class.
/// </summary>
/// <typeparam name="T">The type of the content.</typeparam>
public class SuccessResult<T> : Result
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
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with a specified title.
    /// </summary>
    /// <param name="title">The title associated with the success result.</param>
    public SuccessResult(string title) : base(true)
    {
        Status = StatusCodes.Status200OK;
        Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with a specified title and content.
    /// </summary>
    /// <param name="title">The title associated with the success result.</param>
    /// <param name="content">The content associated with the success result.</param>
    public SuccessResult(string title, T content) : base(true)
    {
        Status = StatusCodes.Status200OK;
        Title = title;
        Content = content;
        AddPaging(content);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with a specified status code and title.
    /// </summary>
    /// <param name="status">The HTTP status code of the success result.</param>
    /// <param name="title">The title associated with the success result.</param>
    public SuccessResult(int status, string title) : base(true)
    {
        Status = status;
        Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with specified status code, title, and content.
    /// </summary>
    /// <param name="status">The HTTP status code of the success result.</param>
    /// <param name="title">The title associated with the success result.</param>
    /// <param name="content">The content associated with the success result.</param>
    public SuccessResult(int status, string title, T content) : base(true)
    {
        Status = status;
        Title = title;
        Content = content;
        AddPaging(content);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with a specified title and message.
    /// </summary>
    /// <param name="title">The title associated with the success result.</param>
    /// <param name="message">The additional message associated with the success result.</param>
    public SuccessResult(string title, string message) : base(true, message)
    {
        Status = StatusCodes.Status200OK;
        Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with a specified status code, title, and message.
    /// </summary>
    /// <param name="status">The HTTP status code of the success result.</param>
    /// <param name="title">The title associated with the success result.</param>
    /// <param name="message">The additional message associated with the success result.</param>
    public SuccessResult(int status, string title, string message) : base(true, message)
    {
        Status = status;
        Title = title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with a specified title, message, and content.
    /// </summary>
    /// <param name="title">The title associated with the success result.</param>
    /// <param name="message">The additional message associated with the success result.</param>
    /// <param name="content">The content associated with the success result.</param>
    public SuccessResult(string title, string message, T content) : base(true, message)
    {
        Status = StatusCodes.Status200OK;
        Title = title;
        Content = content;
        AddPaging(content);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class with a specified status code, title, message, and content.
    /// </summary>
    /// <param name="status">The HTTP status code of the success result.</param>
    /// <param name="title">The title associated with the success result.</param>
    /// <param name="message">The additional message associated with the success result.</param>
    /// <param name="content">The content associated with the success result.</param>
    public SuccessResult(int status, string title, string message, T content) : base(true, message)
    {
        Status = status;
        Title = title;
        Content = content;
        AddPaging(content);
    }

    /// <summary>
    /// Adds paging information based on the provided content.
    /// </summary>
    /// <param name="content">The content with paging information.</param>
    private void AddPaging(T content)
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

    /// <summary>
    /// The content associated with the success result.
    /// </summary>
    public T Content { get; set; }

    /// <summary>
    /// The paging information associated with the success result.
    /// </summary>
    public Paging Paging { get; set; }
}
