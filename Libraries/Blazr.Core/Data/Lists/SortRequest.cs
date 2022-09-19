﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record SortRequest
{
    public string? SortField { get; init; }

    public bool SortDescending { get; init; }

    public bool IsSorting => !string.IsNullOrWhiteSpace(SortField);
}
