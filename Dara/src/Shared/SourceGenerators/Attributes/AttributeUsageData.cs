using System;

namespace Dara.Shared.SourceGenerators.Attributes;

public record AttributeUsageData(AttributeTargets Targets, bool AllowMultiple = false, bool Inherited = false);