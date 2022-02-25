using BoardTools;
using Content;
using Ship;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Drea Renthal",
                    "Pirate Lord",
                    Faction.Scum,
                    4,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DreaRenthalAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Astromech,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
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
            Combat.Attacker.AddAvailableDiceModification
            (
                new DiceModificationAction()
                {
                    ImageUrl = HostShip.ImageUrl
                },
                HostShip
            );
        }

        private class DiceModificationAction : ActionsList.GenericAction
        {
            public DiceModificationAction()
            {
                Name = DiceModificationName = "Drea Renthal";
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
