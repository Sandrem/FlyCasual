﻿using Ship;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class KathScarletSV : FiresprayClassPatrolCraft
        {
            public KathScarletSV() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kath Scarlet",
                    4,
                    72,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KathScarletAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 151
                );

                ModelInfo.SkinName = "Kath Scarlet";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KathScarletAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckAbility;
        }

        private void CheckAbility(ref int count)
        {
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) return;

            if (Combat.Defender.ShipsBumped.Any(s => s.Owner.PlayerNo == HostShip.Owner.PlayerNo && !s.PilotInfo.IsLimited))
            {
                Messages.ShowInfo("A non-limited friendly ship bumping the defender gives " + HostShip.PilotInfo.PilotName + " +1 attack die");
                count++;
            }
        }
    }
}