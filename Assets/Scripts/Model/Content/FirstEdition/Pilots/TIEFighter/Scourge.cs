using Abilities.FirstEdition;
using Arcs;
using BoardTools;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class Scourge : TIEFighter
        {
            public Scourge()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Scourge\"",
                    7,
                    17,
                    isLimited: true,
                    abilityType: typeof(ScourgeAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
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

        protected virtual void CheckConditions()
        {
            if (Combat.Defender.Damage.DamageCards.Any())
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            }
        }

        protected virtual void SendExtraDiceMessage()
        {
            Messages.ShowInfo("Defender has a damage card. Roll an additional attack die.");
        }

        protected void RollExtraDice(ref int count)
        {
            count++;
            SendExtraDiceMessage();
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }
    }
}