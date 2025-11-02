using Ambev.DeveloperEvaluation.Common.Config;

namespace Ambev.DeveloperEvaluation.Application.Config;

public class SaleCancelledIntegrationKafkaConfig : KafkaConfig
{
    public override string TopicName { get { return "AMBEV.Integration.API.SaleCancelled"; } }
    public override string GroupName { get { return "AMBEV.Integration.API.SaleCancelled-group"; } }
    public override string TopicError { get { return "AMBEV.Integration.API.SaleCancelled_Error"; } }
}