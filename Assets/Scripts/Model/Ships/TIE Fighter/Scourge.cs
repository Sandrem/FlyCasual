using System.Linq;

namespace Ship
{
    namespace TIEFighter
    {
        public class Scourge : TIEFighter
        {
            public Scourge()
            {
                PilotName = "\"Scourge\"";
                PilotSkill = 7;
                Cost = 17;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.ScourgeAbility());
            }
        }
    }
}

namespace Abilities
{
    public class ScourgeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditions;
        }

        private void CheckConditions()
        {
            if (Combat.Defender.Damage.DamageCards.Any())
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            }
        }

        private void RollExtraDice(ref int count)
        {
            count++;
            Messages.ShowInfo("Defender has a Damage card. Roll 1 additional attack die.");
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }
    }
}
