using Arcs;
using Ship;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class Quickdraw : TIESfFighter
        {
            public Quickdraw() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Quickdraw\"",
                    6,
                    45,
                    isLimited: true,
                    extraUpgradeIcon: UpgradeType.Talent,
                    abilityType: typeof(Abilities.SecondEdition.QuickdrawAbility),
                    charges: 1,
                    regensCharges: true
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/d038dadd7a62bbe2de89d3866e1a3639.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class QuickdrawAbility : Abilities.FirstEdition.QuickDrawPilotAbility
    {
        protected override bool IsAbilityCanBeUsed()
        {
            if (HostShip.State.Charges == 0 || HostShip.IsCannotAttackSecondTime) return false;

            return true;
        }

        protected override void MarkAbilityAsUsed()
        {
            HostShip.SpendCharge();
        }
    }
}
