using Ambev.DeveloperEvaluation.Common.Config;

namespace Ambev.DeveloperEvaluation.Application.Config;

public class SaleItemCancelledIntegrationKafkaConfig : KafkaConfig
{
    public override string TopicName { get { return "AMBEV.Integration.API.SaleItemCancelled"; } }
    public override string GroupName { get { return "AMBEV.Integration.API.SaleItemCancelled-group"; } }
    public override string TopicError { get { return "AMBEV.Integration.API.SaleItemCancelled_Error"; } }
}