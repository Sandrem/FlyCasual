using Ship;
using System.Collections.Generic;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Kath Scarlet",
                    "Captain of the Binayre Pirates",
                    Faction.Scum,
                    4,
                    7,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KathScarletAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    seImageNumber: 151,
                    skinName: "Kath Scarlet"
                );
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