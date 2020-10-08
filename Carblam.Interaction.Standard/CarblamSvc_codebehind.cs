

using System;

public partial class OrderHistoryInfoType
{
    public DateTime? Created
    {
        get { return createdTicksFieldSpecified ? new DateTime(createdTicksField) : new Nullable<DateTime>(); }
        set { createdTicksFieldSpecified = value.HasValue; if (value.HasValue) createdTicksField = value.Value.Ticks; }
    }

    public DateTime? Confirmed
    {
        get { return confirmedTicksFieldSpecified ? new DateTime(confirmedTicksField) : new Nullable<DateTime>(); }
        set { confirmedTicksFieldSpecified = value.HasValue; if (value.HasValue) confirmedTicksField = value.Value.Ticks; }
    }

    public DateTime? Accepted
    {
        get { return acceptedTicksFieldSpecified ? new DateTime(acceptedTicksField) : new Nullable<DateTime>(); }
        set { acceptedTicksFieldSpecified = value.HasValue; if (value.HasValue) acceptedTicksField = value.Value.Ticks; }
    }

    public DateTime? Loading
    {
        get { return loadingTicksFieldSpecified ? new DateTime(loadingTicksField) : new Nullable<DateTime>(); }
        set { loadingTicksFieldSpecified = value.HasValue; if (value.HasValue) loadingTicksField = value.Value.Ticks; }
    }

    public DateTime? Loaded
    {
        get { return loadedTicksFieldSpecified ? new DateTime(loadedTicksField) : new Nullable<DateTime>(); }
        set { loadedTicksFieldSpecified = value.HasValue; if (value.HasValue) loadedTicksField = value.Value.Ticks; }
    }

    public DateTime? Delivered
    {
        get { return deliveredTicksFieldSpecified ? new DateTime(deliveredTicksField) : new Nullable<DateTime>(); }
        set { deliveredTicksFieldSpecified = value.HasValue; if (value.HasValue) deliveredTicksField = value.Value.Ticks; }
    }

    public DateTime? Finished
    {
        get { return finishedTicksFieldSpecified ? new DateTime(finishedTicksField) : new Nullable<DateTime>(); }
        set { finishedTicksFieldSpecified = value.HasValue; if (value.HasValue) finishedTicksField = value.Value.Ticks; }
    }
    
    public DateTime? Cancelled
    {
        get { return cancelledTicksFieldSpecified ? new DateTime(cancelledTicksField) : new Nullable<DateTime>(); }
        set { cancelledTicksFieldSpecified = value.HasValue; if (value.HasValue) cancelledTicksField = value.Value.Ticks; }
    }
}
