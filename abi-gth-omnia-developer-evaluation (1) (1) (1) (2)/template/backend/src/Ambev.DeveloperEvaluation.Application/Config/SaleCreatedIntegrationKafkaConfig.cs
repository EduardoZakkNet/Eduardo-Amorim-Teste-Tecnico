using System.Diagnostics.CodeAnalysis;
using Ambev.DeveloperEvaluation.Common.Config;

namespace Ambev.DeveloperEvaluation.Application.Config;

[ExcludeFromCodeCoverage]
public class SaleCreatedIntegrationKafkaConfig : KafkaConfig
{
    public override string? TopicName { get { return "AMBEV.Integration.API.SaleCreated"; } }
    public override string GroupName { get { return "AMBEV.Integration.API.SaleCreated-group"; } }
    public override string TopicError { get { return "AMBEV.Integration.API.SaleCreated_Error"; } }
}