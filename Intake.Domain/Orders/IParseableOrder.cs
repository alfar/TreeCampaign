using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders;

public interface IParseableOrder
{
    OrderId Id { get; }
    CampaignRef CampaignId { get; }
    string Message { get; }
    ValidatedOrder Accept(ValidationSuccess validationResult);
}