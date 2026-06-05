using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Application.BackgroundWorkers.Signals;

public abstract record ValidationSignalBase();
public sealed record EverythingValidationSignal() : ValidationSignalBase;
public sealed record CampaignValidationSignal(CampaignRef CampaignId) : ValidationSignalBase;
public sealed record OrderValidationSignal(OrderId OrderId) : ValidationSignalBase;
