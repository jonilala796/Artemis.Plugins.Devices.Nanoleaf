using System;

namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Attributes;

/// <inheritdoc />
/// <summary>
/// Represents the side length of a Nanoleaf device.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class SideLengthAttribute : Attribute
{
    #region Properties & Fields

    /// <summary>
    /// Gets the side length of the Nanoleaf device.
    /// </summary>
    internal float SideLength { get; }

    #endregion

    #region Constructors

    /// <inheritdoc />
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Attributes.SideLengthAttribute" /> class.
    /// </summary>
    /// <param name="sideLength">The side length of the Nanoleaf device.</param>
    public SideLengthAttribute(float sideLength)
    {
        this.SideLength = sideLength;
    }

    #endregion
}