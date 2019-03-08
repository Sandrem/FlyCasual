using BoardTools;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class DreaRenthal : BTLA4YWing
        {
            public DreaRenthal() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Drea Renthal",
                    4,
                    42,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DreaRenthalAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Illicit },
                    factionOverride: Faction.Scum,
                    seImageNumber: 166
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DreaRenthalAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddDreaRenthalAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddDreaRenthalAbility;
        }

        private void AddDreaRenthalAbility(GenericShip ship)
        {
            Combat.Attacker.AddAvailableDiceModification(new DiceModificationAction() { HostShip = Roster.GetShipById("ShipId:" + this.HostShip.ShipId) });
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
                if (Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo && !Combat.Attacker.PilotInfo.IsLimited && Board.IsShipInArc(HostShip, Combat.Defender))
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
