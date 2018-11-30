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
                    74,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KathScarletAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
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
            if (Combat.ChosenWeapon != HostShip.PrimaryWeapon) return;

            if (Combat.Defender.ShipsBumped.Any(s => s.Owner.PlayerNo == HostShip.Owner.PlayerNo && !s.PilotInfo.IsLimited))
            {
                Messages.ShowInfo("Kath Scarlet: +1 attack die");
                count++;
            }
        }
    }
}