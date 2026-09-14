using Common.Domain.Abstractions;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Domain.TeamMembers;
using TreeCampaign.Domain.TeamMembers.ValueObjects;
using TreeCampaign.Domain.Teams.Events;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams;

public abstract class TeamBase : IHasDomainEvents
{
    public required TeamId Id { get; init; }
    public TeamName Name { get; protected set; } = TeamName.Empty;
    public required CampaignId CampaignId { get; init; }
    public TeamStatus Status { get; private set; } = TeamStatus.Active;
    public TreeCount CurrentExtraTrees { get; private set; } = TreeCount.From(0);
    public TreeCount TotalExtraTrees { get; private set; } = TreeCount.From(0);

    private readonly List<TeamMember> _members = [];
    public IReadOnlyCollection<TeamMember> Members => _members.AsReadOnly();

    private readonly List<IDomainEvent> _newEvents = [];
    public IReadOnlyCollection<IDomainEvent> NewEvents => _newEvents.AsReadOnly();

    protected TeamBase() { }

    protected void Raise(IDomainEvent domainEvent) => _newEvents.Add(domainEvent);

    public void UpdateName(TeamName name)
    {
        Name = name;
        Raise(new TeamNameUpdated(Id, CampaignId, name));
    }

    public void AddMember(string name, string? scoutRelativeName, string? phoneNumber)
    {
        _members.Add(new TeamMember { Id = new TeamMemberId(Guid.NewGuid()), Name = name, ScoutRelativeName = scoutRelativeName, PhoneNumber = phoneNumber, TeamId = Id });
        Raise(new TeamMemberAdded(Id, CampaignId, name, scoutRelativeName, phoneNumber));
    }

    public void RemoveMember(TeamMemberId memberId)
    {
        var member = _members.FirstOrDefault(m => m.Id == memberId);
        if (member is not null)
        {
            _members.Remove(member);
            Raise(new TeamMemberRemoved(Id, CampaignId, memberId));
        }
    }

    public void GoOnBreak()
    {
        Status = TeamStatus.OnBreak;
        Raise(new TeamWentOnBreak(Id, CampaignId));
    }

    public void ResumeFromBreak()
    {
        Status = TeamStatus.Active;
        Raise(new TeamResumedFromBreak(Id, CampaignId));
    }

    public void AdjustExtraTrees(TreeCountDelta delta)
    {
        var newCurrent = CurrentExtraTrees.Adjust(delta);
        var actualDelta = TreeCountDelta.From(newCurrent.Value - CurrentExtraTrees.Value);
        if (actualDelta.Value == 0) return;

        CurrentExtraTrees = newCurrent;
        TotalExtraTrees = TotalExtraTrees.Adjust(actualDelta);
        Raise(new TeamExtraTreesAdjusted(Id, CampaignId, actualDelta, CurrentExtraTrees, TotalExtraTrees));
    }

    public void ResetCurrentExtraTrees()
    {
        if (CurrentExtraTrees.Value == 0) return;
        CurrentExtraTrees = TreeCount.From(0);
        Raise(new TeamExtraTreesReset(Id, CampaignId, TotalExtraTrees));
    }

    public void ClearEvents() => _newEvents.Clear();
}
