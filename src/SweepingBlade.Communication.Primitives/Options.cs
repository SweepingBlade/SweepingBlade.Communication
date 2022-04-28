using System;
using SweepingBlade.Communication.Primitives.Channels;
using SweepingBlade.Communication.Primitives.Serialization;

namespace SweepingBlade.Communication.Primitives;

public abstract class Options
{
    protected abstract IConnectionFactory GetConnectionFactory();
    protected abstract IChannelFactory GetDefaultChannelFactory();
    protected abstract IDescriptionResolver GetDescriptionResolver();

    private IChannelFactory _channelFactory;
    private IConnectionFactory _connectionFactory;
    private IDescriptionResolver _descriptionResolver;

    public IChannelFactory ChannelFactory => _channelFactory ??= GetDefaultChannelFactory();
    public TimeSpan CloseTimeout { get; set; } = TimeSpan.FromSeconds(5);

    public IConnectionFactory ConnectionFactory => _connectionFactory ??= GetConnectionFactory();
    public IDescriptionResolver DescriptionResolver => _descriptionResolver ??= GetDescriptionResolver();
    public TimeSpan OpenTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan ReceiveTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan SendTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public ISerializer Serializer { get; }

    protected Options(ISerializer serializer)
    {
        Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }
}