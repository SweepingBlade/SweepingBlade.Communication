using System;
using System.ComponentModel;
using SweepingBlade.Communication.NamedPipe.Channels;
using SweepingBlade.Communication.Primitives;
using SweepingBlade.Communication.Primitives.Channels;
using SweepingBlade.Communication.Primitives.Serialization;

namespace SweepingBlade.Communication.NamedPipe;

public class NamedPipeOptions : Options
{
    private readonly NamedPipeSecurityMode _securityMode;

    public int InputBufferSize { get; set; } = ushort.MaxValue;

    public int MaxConnections { get; set; } = -1;

    public int OutputBufferSize { get; set; } = ushort.MaxValue;

    public NamedPipeOptions(ISerializer serializer, NamedPipeSecurityMode securityMode)
        : base(serializer)
    {
        if (!Enum.IsDefined(typeof(NamedPipeSecurityMode), securityMode)) throw new InvalidEnumArgumentException(nameof(securityMode), (int)securityMode, typeof(NamedPipeSecurityMode));
        _securityMode = securityMode;
    }

    protected override IConnectionFactory GetConnectionFactory()
    {
        return new NamedPipeConnectionFactory(_securityMode, OpenTimeout, CloseTimeout, SendTimeout, ReceiveTimeout, MaxConnections, InputBufferSize, OutputBufferSize);
    }

    protected override IChannelFactory GetDefaultChannelFactory()
    {
        return new NamedPipeChannelFactory(this);
    }

    protected override IDescriptionResolver GetDescriptionResolver()
    {
        return new DescriptionResolver();
    }
}