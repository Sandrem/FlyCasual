using System;
using System.Collections.Generic;
using Ship;

public enum CriticalCardType
{
    Ship,
    Pilot
}

public class GenericDamageCard
{
    public GenericShip Host;

    public string Name;
    public CriticalCardType Type;
    public bool IsFaceup;
    public bool AiAvoids;

    public List<DieSide> CancelDiceResults = new List<DieSide>();

    private string imageUrl;
    public string ImageUrl
    {
        get { return imageUrl ?? ImageUrls.GetImageUrl(this); }
        set { imageUrl = value; }
    }

    private int damageValue;
    public int DamageValue
    {
        get { return (IsFaceup) ? damageValue : 1; }
        protected set { damageValue = value; }
    }

    public GenericDamageCard()
    {
        DamageValue = 1;
    }

    public void Assign(GenericShip host, Action callback)
    {
        Host = host;

        if (IsFaceup)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Apply critical hit card effect",
                TriggerType = TriggerTypes.OnFaceupCritCardIsDealt,
                TriggerOwner = host.Owner.PlayerNo,
                EventHandler = ApplyEffect
            });

            Triggers.ResolveTriggers(TriggerTypes.OnFaceupCritCardIsDealt, callback);
        }
        else
        {
            callback();
        }
    }

    public virtual void ApplyEffect(object sender, EventArgs e)
    {

    }

    public virtual void DiscardEffect()
    {

    }

    protected void CallAddCancelCritAction(GenericShip ship)
    {
        AddCancelCritAction();
    }

    protected void AddCancelCritAction()
    {
        ActionsList.CancelCritAction cancelCritAction = new ActionsList.CancelCritAction();
        cancelCritAction.Initilize(this);
        Host.AddAvailableAction(cancelCritAction);
    }

}

