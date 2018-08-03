using Abilities.SecondEdition;
using BoardTools;
using RuleSets;
using Ship;

namespace Ship
{
    namespace YWing
    {
        public class DreaRenthal : YWing, ISecondEditionPilot
        {
            public DreaRenthal() : base()
            {
                PilotName = "Drea Renthal";
                PilotSkill = 4;
                Cost = 40;

                IsUnique = true;

                faction = Faction.Scum;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new DreaRenthalAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // empty unneeded bam
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DreaRenthalAbilitySE : GenericAbility
    {

        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddDreaRenthalAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddDreaRenthalAbility;
        }

        private void AddDreaRenthalAbility(GenericShip ship)
        {
            Combat.Attacker.AddAvailableDiceModification(new DiceModificationAction() { Host = this.HostShip });
        }

        private class DiceModificationAction : ActionsList.GenericAction
        {
            public DiceModificationAction()
            {
                Name = DiceModificationName = "Drea Renthal's ability";
                IsReroll = true;
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (Combat.Attacker.Owner.PlayerNo == Host.Owner.PlayerNo && !Combat.Attacker.IsUnique && Board.IsShipInArc(Host, Combat.Defender))
                {
                    return true;
                }

                return result;
            }

            public override int GetDiceModificationPriority()
            {
                return 90;
            }

            public override void ActionEffect(System.Action callBack)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = 1,
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }
        }
    }
}