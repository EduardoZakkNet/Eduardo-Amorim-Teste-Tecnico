using Ambev.DeveloperEvaluation.Common.Config;

namespace Ambev.DeveloperEvaluation.Application.Config;

public class SaleModifiedIntegrationKafkaConfig : KafkaConfig
{
    public override string TopicName { get { return "AMBEV.Integration.API.SaleModified"; } }
    public override string GroupName { get { return "AMBEV.Integration.API.SaleModified-group"; } }
    public override string TopicError { get { return "AMBEV.Integration.API.SaleModified_Error"; } }
}