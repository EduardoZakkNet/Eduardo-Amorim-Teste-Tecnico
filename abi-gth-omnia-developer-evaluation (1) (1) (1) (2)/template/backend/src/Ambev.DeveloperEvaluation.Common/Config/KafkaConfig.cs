using System.Diagnostics.CodeAnalysis;

namespace Ambev.DeveloperEvaluation.Common.Config;

[ExcludeFromCodeCoverage]
public abstract class KafkaConfig
{
    public string? Server { get; set; }

    public int TimeOut { get; set; }

    public string? TopicLogs { get; set; }

    public virtual string? TopicName { get; set; }

    public virtual string? GroupName { get; set; }

    public virtual string? TopicError { get; set; }
}
