// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.EntityFrameworkCore.Metadata.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class PropertyParameterBindingFactory : IPropertyParameterBindingFactory
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual ParameterBinding? FindParameter(
        IEntityType entityType,
        Type parameterType,
        string parameterName)
        => FindParameter(entityType.GetProperties(), entityType.GetComplexProperties(), parameterType, parameterName);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual ParameterBinding? FindParameter(
        IComplexType complexType,
        Type parameterType,
        string parameterName)
        => FindParameter(complexType.GetProperties(), complexType.GetComplexProperties(), parameterType, parameterName);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    private static ParameterBinding? FindParameter(
        IEnumerable<IProperty> properties,
        IEnumerable<IComplexProperty> complexProperties,
        Type parameterType,
        string parameterName)
    {
        var candidateNames = GetCandidatePropertyNames(parameterName);

        foreach (var property in properties)
        {
            if (property.ClrType != parameterType)
            {
                continue;
            }

            foreach (var name in candidateNames)
            {
                if (name.Equals(property.Name, StringComparison.Ordinal))
                {
                    return new PropertyParameterBinding(property);
                }
            }
        }

        foreach (var property in complexProperties)
        {
            if (!property.IsCollection
                && property.ClrType == parameterType
                && candidateNames.Contains(property.Name, StringComparer.Ordinal))
            {
                return new ComplexPropertyParameterBinding(property);
            }
        }

        return null;
    }

    private static List<string> GetCandidatePropertyNames(string parameterName)
    {
        var pascalized = char.ToUpperInvariant(parameterName[0]) + parameterName[1..];

        return
        [
            parameterName,
            pascalized,
            "_" + parameterName,
            "_" + pascalized,
            "m_" + parameterName,
            "m_" + pascalized
        ];
    }
}
