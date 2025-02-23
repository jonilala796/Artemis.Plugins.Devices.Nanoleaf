#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
// ReSharper disable UnusedMember.Global

using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Attributes;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Enum;

/// <summary>
/// Represents a specific shape of a Nanoleaf device.
/// </summary>
public enum NanoleafShapeType
{
    [SideLength(150)] [Shape(Shape.Custom)] [ExtControlVersion(ExtControlVersion.v1)]
    Triangle = 0,

    [ExtControlVersion(ExtControlVersion.v1)]
    Rhythm = 1,

    [SideLength(100)] [Shape(Shape.Rectangle)] [ExtControlVersion(ExtControlVersion.v2)]
    Square = 2,

    [SideLength(100)] [Shape(Shape.Rectangle)] [ExtControlVersion(ExtControlVersion.v2)]
    ControlSquareMaster = 3,

    [SideLength(100)] [Shape(Shape.Rectangle)] [ExtControlVersion(ExtControlVersion.v2)]
    ControlSquarePassive = 4,
    [SideLength(67)] [Shape(Shape.Custom)] ShapesHexagon = 7,

    [SideLength(134)] [Shape(Shape.Custom)]
    ShapesTriangle = 8,
    [SideLength(67)] [Shape(Shape.Custom)] ShapesMiniTriangle = 9,
    ShapesController = 12,

    [SideLength(134)] [Shape(Shape.Custom)]
    ElementsHexagon = 14,

    [SideLength(33.5f)] [Shape(Shape.Custom)]
    ElementsHexagonCorner = 15,
    [SideLength(11)] [Shape(Shape.Custom)] LinesConnector = 16,

    [SideLength(154)] [Shape(Shape.Rectangle)]
    LinesLight = 17,

    [SideLength(77)] [Shape(Shape.Rectangle)]
    LinesLightSingleZone = 18,
    [SideLength(11)] [Shape(Shape.Custom)] LinesControllerCap = 19,
    [SideLength(11)] [Shape(Shape.Custom)] LinesPowerConnector = 20,

    [SideLength(50)] [Shape(Shape.Rectangle)]
    Lightstrip4D = 29,

    [SideLength(180)] [Shape(Shape.Rectangle)]
    SkylightPanel = 30,

    [SideLength(180)] [Shape(Shape.Rectangle)]
    SkylightControllerPrimary = 31,

    [SideLength(180)] [Shape(Shape.Rectangle)]
    SkylightControllerPassive = 32,
}