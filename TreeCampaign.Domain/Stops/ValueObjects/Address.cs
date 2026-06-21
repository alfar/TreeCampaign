using TreeCampaign.Domain.ExternalReferences;

namespace TreeCampaign.Domain.Stops.ValueObjects;

public record Address(string DisplayName, decimal Latitude, decimal Longitude, StreetSectionRef StreetSectionId);
