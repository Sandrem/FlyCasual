using Abilities.SecondEdition;
using RuleSets;
using Ship;
using System;

namespace Ship
{
    namespace TIEAdvPrototype
    {
        public class SeventhSister : TIEAdvPrototype, ISecondEditionPilot
        {
            public SeventhSister() : base()
            {
                PilotName = "Seventh Sister";
                PilotSkill = 4;
                Cost = 48;
                MaxForce = 2;

                PilotRuleType = typeof(SecondEdition);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                PilotAbilities.Add(new SeventhSisterAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Elite);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Force);
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SeventhSisterAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults += SeventhSisterDiceMofication;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults -= SeventhSisterDiceMofication;
        }

        private void SeventhSisterDiceMofication(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.SecondEdition.SeventhSisterDiceModification()
            {
                ImageUrl = HostShip.ImageUrl,
                Host = host,
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}


namespace ActionsList.SecondEdition
{
    public class SeventhSisterDiceModification : GenericAction
    {
        public SeventhSisterDiceModification()
        {
            Name = DiceModificationName = "Seventh Sister's ability";
            DiceModificationTiming = DiceModificationTimingType.CompareResults;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.DiceRollDefence.Successes <= Combat.DiceRollAttack.Successes) result = 100;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.DiceRollDefence.Successes > 0 && Combat.ShotInfo.InArc && Combat.Attacker.Force == 2)
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollDefence.ChangeOne(DieSide.Success, DieSide.Blank, false);
            Combat.Attacker.Force -= 2;
            callBack();
        }

    }
}