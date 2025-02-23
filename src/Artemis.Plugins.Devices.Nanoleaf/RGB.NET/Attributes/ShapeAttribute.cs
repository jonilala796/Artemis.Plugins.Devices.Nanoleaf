using System;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ShapeAttribute : Attribute
{
    internal Shape Shape { get; }
    
    public ShapeAttribute(Shape shape)
    {
        this.Shape = shape;
    }
}