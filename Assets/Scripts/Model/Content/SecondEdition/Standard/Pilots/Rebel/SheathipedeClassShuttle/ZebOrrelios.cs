using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.SheathipedeClassShuttle
    {
        public class ZebOrrelios : SheathipedeClassShuttle
        {
            public ZebOrrelios() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Zeb\" Orrelios",
                    "Spectre-4",
                    Faction.Rebel,
                    2,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ZebOrreliosPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.Spectre
                    },
                    seImageNumber: 40
                );

                PilotNameCanonical = "zeborrelios-sheathipedeclassshuttle";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ZebOrreliosPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckCancelCritsFirst += CancelCritsFirstIfDefender;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckCancelCritsFirst -= CancelCritsFirstIfDefender;
        }

        private void CancelCritsFirstIfDefender(GenericShip ship)
        {
            if (ship.ShipId == Combat.Defender.ShipId)
            {
                Combat.DiceRollAttack.CancelCritsFirst = true;
            }
        }
    }
}
