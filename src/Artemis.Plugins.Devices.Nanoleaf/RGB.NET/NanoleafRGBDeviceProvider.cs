﻿using System;
using System.Collections.Generic;
using System.Threading;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.API;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Enum;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Generic;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Helper;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET;

/// <inheritdoc />
/// <summary>
/// Represents a device provider responsible for Nanoleaf devices.
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class NanoleafRGBDeviceProvider : AbstractRGBDeviceProvider
{
    #region Constants

    private const int HEARTBEAT_TIMER = 100;

    #endregion

    #region Properties & Fields

    // ReSharper disable once InconsistentNaming
    private static readonly Lock _lock = new();

    private static NanoleafRGBDeviceProvider? _instance;

    /// <summary>
    /// Gets the singleton <see cref="NanoleafRGBDeviceProvider"/> instance.
    /// </summary>
    public static NanoleafRGBDeviceProvider Instance
    {
        get
        {
            lock (_lock)
                return _instance ?? new NanoleafRGBDeviceProvider();
        }
    }

    /// <summary>
    /// Gets a list of all defined device-definitions.
    /// </summary>
    public List<INanoleafDeviceDefinition> DeviceDefinitions { get; } = [];

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NanoleafRGBDeviceProvider"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if this constructor is called even if there is already an instance of this class.</exception>
    public NanoleafRGBDeviceProvider()
    {
        lock (_lock)
        {
            if (_instance != null)
                throw new InvalidOperationException(
                    $"There can be only one instance of type {nameof(NanoleafRGBDeviceProvider)}");
            _instance = this;
        }
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void InitializeSDK()
    {
    }

    /// <inheritdoc />
    protected override IEnumerable<IRGBDevice> LoadDevices()
    {
        int i = 0;
        foreach (INanoleafDeviceDefinition deviceDefinition in DeviceDefinitions)
        {
            IDeviceUpdateTrigger updateTrigger = GetUpdateTrigger(i++);
            var device = CreateNanoleafDevice(deviceDefinition, updateTrigger);
            if (device != null)
                yield return device;
        }
    }


    private static NanoleafRGBDevice? CreateNanoleafDevice(INanoleafDeviceDefinition deviceDefinition,
        IDeviceUpdateTrigger updateTrigger)
    {
        NanoleafInfo? nanoleafInfo = NanoleafAPI.Info(deviceDefinition.Address, deviceDefinition.AuthToken);
        if (nanoleafInfo == null) return null;
        if (nanoleafInfo.Effects.Select == "*ExtControl*") return null;

        var startExtControl = NanoleafAPI.StartExternalControl(deviceDefinition.Address, deviceDefinition.AuthToken,
            nanoleafInfo.PanelLayout.Layout.PositionData[0].ShapeType.GetExtControlVersion());

        return new NanoleafRGBDevice(new NanoleafRGBDeviceInfo(nanoleafInfo), startExtControl.address,
            startExtControl.port, updateTrigger);
    }

    protected override IDeviceUpdateTrigger CreateUpdateTrigger(int id, double updateRateHardLimit) =>
        new DeviceUpdateTrigger(updateRateHardLimit) { HeartbeatTimer = HEARTBEAT_TIMER };

    #endregion
}