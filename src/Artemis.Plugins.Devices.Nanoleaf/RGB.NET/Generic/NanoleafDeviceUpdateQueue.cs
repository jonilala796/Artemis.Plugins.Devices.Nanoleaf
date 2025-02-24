using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Enum;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Generic;

/// <inheritdoc />
/// <summary>
/// Represents the update-queue performing updates for Nanoleaf devices using streaming v2 API.
/// </summary>
internal sealed class NanoleafDeviceUpdateQueue : UpdateQueue
{
    #region Properties & Fields

    /// <summary>
    /// The UDP-Connection used to send data.
    /// </summary>
    private readonly UdpClient _socket;

    /// <summary>
    /// The buffer the UDP-data is stored in.
    /// </summary>
    private byte[] _buffer;

    private readonly ExtControlVersion? _version;
    private readonly Dictionary<LedId, int> _deviceInfoLedIdToIndex;
    private readonly int _ledCount;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NanoleafDeviceUpdateQueue"/> class.
    /// </summary>
    /// <param name="updateTrigger">The update trigger used by this queue.</param>
    /// <param name="address"></param>
    /// <param name="port"></param>
    /// <param name="ledCount"></param>
    /// <param name="version"></param>
    /// <param name="deviceInfoLedIdToIndex"></param>
    public NanoleafDeviceUpdateQueue(IDeviceUpdateTrigger updateTrigger, string address, int port, int ledCount,
        ExtControlVersion? version, Dictionary<LedId, int> deviceInfoLedIdToIndex)
        : base(updateTrigger)
    {
        _version = version;
        _deviceInfoLedIdToIndex = deviceInfoLedIdToIndex;
        _ledCount = ledCount;


        var bufferLength = version switch
        {
            ExtControlVersion.v1 => 1 + (ledCount * 7),
            ExtControlVersion.v2 => 2 + (ledCount * 8),
            _ => 1 + (ledCount * 8)
        };

        _buffer = new byte[bufferLength];
        _buffer[0] = (byte)(ledCount);

        _socket = new UdpClient();
        _socket.Connect(address, port);
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnUpdate(object? sender, CustomUpdateData customData)
    {
        try
        {
            if (customData[CustomUpdateDataIndex.HEARTBEAT] as bool? ?? false)
                Update(Array.Empty<(object key, Color color)>());
            else
                base.OnUpdate(sender, customData);
        }
        catch (Exception ex)
        {
            NanoleafRGBDeviceProvider.Instance.Throw(ex);
        }
    }

    /// <inheritdoc />
    protected override bool Update(ReadOnlySpan<(object key, Color color)> dataSet)
    {
        return _version switch
        {
            ExtControlVersion.v1 => UpdateV1(dataSet),
            ExtControlVersion.v2 => UpdateV2(dataSet),
            _ => UpdateV1(dataSet)
        };
    }

    private bool UpdateV1(ReadOnlySpan<(Object key, Color color)> dataSet)
    {
        try
        {
            Span<byte> data = _buffer.AsSpan();
            data[0] = (byte)(_ledCount);
            foreach ((object key, Color color) in dataSet)
            {
                int ledIndex = ((int)key & 0x000FFFFF) - 1;
                var ledId = _deviceInfoLedIdToIndex[(LedId)key];
                int offset = (ledIndex * 7) + 1;
                data[offset] = (byte)ledId;
                data[offset + 1] = 1; // Number of frames, always 1
                data[offset + 2] = color.GetR();
                data[offset + 3] = color.GetG();
                data[offset + 4] = color.GetB();
                data[offset + 5] = 0; // White LED element, currently ignored
                data[offset + 6] = 0; // transitionTime 
            }

            _socket.Send(_buffer);

            return true;
        }
        catch (Exception ex)
        {
            NanoleafRGBDeviceProvider.Instance.Throw(ex);
        }

        return false;
    }

    private bool UpdateV2(ReadOnlySpan<(Object key, Color color)> dataSet)
    {
        try
        {
            Span<byte> data = _buffer.AsSpan();
            data[0] = (byte)(_ledCount >> 8); // Number of panels high byte
            data[1] = (byte)(_ledCount & 0xFF); // Number of panels low byte
            foreach ((object key, Color color) in dataSet)
            {
                int ledIndex = ((int)key & 0x000FFFFF) - 1;
                var ledId = _deviceInfoLedIdToIndex[(LedId)key];
                int offset = (ledIndex * 8) + 2;
                data[offset] = (byte)(ledId >> 8); // panelId high byte
                data[offset + 1] = (byte)(ledId & 0xFF); // panelId low byte
                data[offset + 2] = color.GetR();
                data[offset + 3] = color.GetG();
                data[offset + 4] = color.GetB();
                data[offset + 5] = 0; // White LED element, currently ignored
                data[offset + 6] = 0; // transitionTime high byte
                data[offset + 7] = 0; // transitionTime low byte
            }

            _socket.Send(_buffer);

            return true;
        }
        catch (Exception ex)
        {
            NanoleafRGBDeviceProvider.Instance.Throw(ex);
        }

        return false;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        base.Dispose();

        _socket.Dispose();
        _buffer = [];
    }

    #endregion
}