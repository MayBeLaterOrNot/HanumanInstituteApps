﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// ReSharper disable CheckNamespace
namespace HanumanInstitute.Services;

/// <summary>
/// Used by ValidateObjectAttribute to hold combined validation results of multiple objects.
/// </summary>
public class CompositeValidationResult : ValidationResult
{
    private readonly List<ValidationResult> _results = new();

    /// <summary>
    /// Returns the combined validation results.
    /// </summary>
    public IEnumerable<ValidationResult> Results => _results;

    public CompositeValidationResult(string errorMessage) : base(errorMessage)
    { }

    public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames)
    { }

    protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult)
    { }

    /// <summary>
    /// Adds validation results to the merged list.
    /// </summary>
    /// <param name="validationResult">The validation results to add.</param>
    public void AddResult(ValidationResult validationResult)
    {
        _results.Add(validationResult);
    }
}
