using Abilities.SecondEdition;
using ActionsList;
using Ship;
using System.Linq;
using Upgrade;
using Tokens;

namespace Ship
{
    namespace SecondEdition.TIEReaper
    {
        public class MajorVermeil : TIEReaper
        {
            public MajorVermeil() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Major Vermeil",
                    4,
                    49,
                    isLimited: true,
                    abilityType: typeof(MajorVermeilAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 113
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MajorVermeilAbility : Abilities.FirstEdition.MajorVermeilAbility
    {
        protected override void AddMajorVermeilModifierEffect(GenericShip ship)
        {
            if (Combat.Attacker.ShipId == ship.ShipId
                && !Combat.Defender.Tokens.HasGreenTokens()
                && (Combat.DiceRollAttack.Focuses > 0 || Combat.DiceRollAttack.Blanks > 0))
            {
                ship.AddAvailableDiceModification(new MajorVermeilAction
                {
                    ImageUrl = HostShip.ImageUrl,
                    Host = HostShip
                });
            }
        }
    }
}