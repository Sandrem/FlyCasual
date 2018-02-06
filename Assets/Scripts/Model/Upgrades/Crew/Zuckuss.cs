using Upgrade;
using Ship;
using System.Linq;
using Tokens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SquadBuilderNS;
using Abilities;

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

            UpgradeAbilities.Add(new ZuckussCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }

    }
}

namespace Abilities
{
    public class ZuckussCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        { 
            HostShip.AfterGenerateAvailableOppositeActionEffectsList += ZuckussCrewAddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableOppositeActionEffectsList -= ZuckussCrewAddAction;
        }
        private void ZuckussCrewAddAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction action = new ActionsList.ZuckussDiceModification()            
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            };
            host.AddAvailableOppositeActionEffect(action);
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
