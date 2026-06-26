namespace TreeCampaign.Domain.Teams.ValueObjects;

public record TeamMember(Guid Id, string Name, string? ScoutRelativeName, string PhoneNumber);
