using System;

namespace TreeCampaign.Domain.Stops;

public abstract class ReopenableStop : StopBase
{
    public UnassignedStop Reopen()
    {
        return UnassignedStop.CreateFrom(this);
    }
}
