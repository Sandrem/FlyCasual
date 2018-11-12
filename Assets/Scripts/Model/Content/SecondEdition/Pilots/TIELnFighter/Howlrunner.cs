using Abilities.SecondEdition;
using Ship;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class Howlrunner : TIELnFighter
        {
            public Howlrunner() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Howlrunner\"",
                    5,
                    40,
                    limited: 1,
                    abilityType: typeof(HowlrunnerAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 81;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //When a friendly ship at range 0-1 performs a primary attack, that ship may reroll one attack die.

    public class HowlrunnerAbility : Abilities.FirstEdition.HowlrunnerAbility
    {
        protected override bool IsAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (Combat.Attacker.Owner != HostShip.Owner) return false;

            if (Combat.ChosenWeapon.GetType() != typeof(PrimaryWeaponClass)) return false;

            BoardTools.DistanceInfo positionInfo = new BoardTools.DistanceInfo(HostShip, Combat.Attacker);
            if (positionInfo.Range > 1) return false;

            return true;
        }
    }
}