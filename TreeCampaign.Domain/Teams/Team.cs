using Common.Domain.Abstractions;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.TeamMembers;
using TreeCampaign.Domain.TeamMembers.ValueObjects;
using TreeCampaign.Domain.Teams.Events;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams;

public class Team : IHasDomainEvents
{
    public required TeamId Id { get; init; }
    public TeamName Name { get; private set; } = TeamName.Empty;
    public required CampaignId CampaignId { get; init; }
    public TeamStatus Status { get; private set; } = TeamStatus.Active;

    private readonly List<TeamMember> _members = [];
    public IReadOnlyCollection<TeamMember> Members => _members.AsReadOnly();

    private readonly List<IDomainEvent> _newEvents = [];
    public IReadOnlyCollection<IDomainEvent> NewEvents => _newEvents.AsReadOnly();

    private Team() { }

    public static Team Create(CampaignId campaignId, TeamName name)
    {
        return new Team
        {
            Id = new TeamId(Guid.NewGuid()),
            Name = name,
            CampaignId = campaignId,
        };
    }

    public void UpdateName(TeamName name)
    {
        Name = name;
    }

    public void AddMember(string name, string? scoutRelativeName, string phoneNumber)
    {
        _members.Add(new TeamMember { Id = new TeamMemberId(Guid.NewGuid()), Name = name, ScoutRelativeName = scoutRelativeName, PhoneNumber = phoneNumber, TeamId = Id });
    }

    public void RemoveMember(TeamMemberId memberId)
    {
        var member = _members.FirstOrDefault(m => m.Id == memberId);
        if (member is not null)
            _members.Remove(member);
    }

    public void GoOnBreak()
    {
        Status = TeamStatus.OnBreak;
        _newEvents.Add(new TeamWentOnBreak(Id));
    }

    public void ResumeFromBreak()
    {
        if (Status != TeamStatus.OnBreak) return;
        Status = TeamStatus.Active;
        _newEvents.Add(new TeamResumedFromBreak(Id));
    }

    public void ReportTrailerFull()
    {
        Status = TeamStatus.TrailerFull;
        _newEvents.Add(new TeamReportedTrailerFull(Id));
    }

    public void ClearTrailerFull()
    {
        if (Status != TeamStatus.TrailerFull) return;
        Status = TeamStatus.Active;
        _newEvents.Add(new TeamTrailerCleared(Id));
    }

    public void ClearEvents() => _newEvents.Clear();
}
