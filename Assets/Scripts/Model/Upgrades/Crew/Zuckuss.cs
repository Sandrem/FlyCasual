using Upgrade;
using Ship;
using System.Linq;
using Tokens;
using System.Collections.Generic;
using UnityEngine;
using SquadBuilderNS;

namespace UpgradesList
{
    public class Zuckuss : GenericUpgrade
    {
        public Zuckuss() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Zuckuss";
            Cost = 1;

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableOppositeActionEffectsList += ZuckussDiceModification;
        }

        private void ZuckussDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.ZuckussDiceModification() { ImageUrl = ImageUrl, Host = this.Host };
            host.AddAvailableOppositeActionEffect(newAction);
        }
    }
}

namespace ActionsList
{

    public class ZuckussDiceModification : GenericAction
    {

        public ZuckussDiceModification()
        {
            Name = EffectName = "Zuckuss";

            IsReroll = true;
            IsOpposite = true;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Defence) && (!Host.Tokens.HasToken(typeof(StressToken)))) result = true;
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if ((Combat.AttackStep == CombatStep.Defence) && (!Host.Tokens.HasToken(typeof(StressToken))))
            {
                int defenceSuccesses = Combat.DiceRollDefence.RegularSuccesses;

                if (Combat.Defender.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    if (defenceSuccesses > 0) result = 90;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Host.OnRerollIsConfirmed += AssignStressForEachRerolled;

            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

        private void AssignStressForEachRerolled(GenericShip ship)
        {
            int diceRerolledCount = DiceRerollManager.CurrentDiceRerollManager.GetDiceReadyForReroll().Count();

            for (int i = 0; i < diceRerolledCount; i++)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Zuckuss: Assign stress for each rerolled dice",
                    TriggerType = TriggerTypes.OnRerollIsConfirmed,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = AssingStress,
                    Skippable = true
                });
            }

            Host.OnRerollIsConfirmed -= AssignStressForEachRerolled;
        }

        private void AssingStress(object sender, System.EventArgs e)
        {
            Messages.ShowError("Zuckuss: Get Stress token");
            Host.Tokens.AssignToken(new StressToken(Host), Triggers.FinishTrigger);
        }

    }

}
