// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Saharaviewpoint.Core.Services;

public abstract class BaseService
{
    protected static string GenerateCacheKey<TModel>(TModel model, params object[] additionalValues)
    {
        var stringBuilder = new StringBuilder();

        // Use reflection to get properties of the model
        PropertyInfo[] properties = typeof(TModel).GetProperties();

        bool isFirst = true;
        foreach (var property in properties)
        {
            if (property.GetValue(model) == null)
                continue;
            AppendValueToStringBuilder(ref isFirst, stringBuilder, property.GetValue(model));
        }

        // Handle additional values
        foreach (object value in additionalValues)
        {
            if (value == null)
                continue;
            AppendValueToStringBuilder(ref isFirst, stringBuilder, value);
        }

        return stringBuilder.ToString();
    }

    protected static string GenerateCacheKey(params object[] values)
    {
        var stringBuilder = new StringBuilder();

        bool isFirst = true;
        foreach (object value in values)
        {
            if (value == null)
                continue;
            AppendValueToStringBuilder(ref isFirst, stringBuilder, value);
        }

        return stringBuilder.ToString();
    }

    private static void AppendValueToStringBuilder(ref bool isFirst, StringBuilder stringBuilder, object value)
    {
        if (!isFirst)
        {
            stringBuilder.Append("-");
        }
        isFirst = false;

        // Handle DateTime values
        if (value is DateTime dateTimeValue)
        {
            stringBuilder.Append(dateTimeValue.ToString("o"));
        }
        // Handle Enum values
        else if (value is Enum enumValue)
        {
            stringBuilder.Append(enumValue);
        }
        // Handle Collections (Arrays, Lists, etc.)
        else if (value is IEnumerable enumerableValue && !(value is string))
        {
            stringBuilder.Append(string.Join(",", enumerableValue.Cast<object>().Select(e => e?.ToString())));
        }
        // Handle primitive types, strings, and Guids
        else if (value != null && value.GetType().IsPrimitive || value is decimal || value is string || value is Guid)
        {
            stringBuilder.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
        }
        // Handle complex objects, potentially recursively or by a unique identifier
        else if (value != null)
        {
            stringBuilder.Append(value.GetType().Name);
        }
    }
}