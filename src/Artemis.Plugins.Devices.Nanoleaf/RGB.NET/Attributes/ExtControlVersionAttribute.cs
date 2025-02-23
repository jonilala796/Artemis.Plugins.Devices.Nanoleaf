using System;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Enum;

namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Attributes;


[AttributeUsage(AttributeTargets.Field)]
public class ExtControlVersionAttribute : Attribute
{
   internal ExtControlVersion ExtControlVersion { get; }

   public ExtControlVersionAttribute(ExtControlVersion extControlVersion)
   {
      ExtControlVersion = extControlVersion;
   }
}