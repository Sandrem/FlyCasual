using Bombs;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScurrgH6Bomber
    {
        public class CaptainNym : ScurrgH6Bomber
        {
            public CaptainNym() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Nym",
                    5,
                    52,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.CaptainNymScumAbiliity),
                    charges: 1,
                    regensCharges: true
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 204;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainNymScumAbiliity : Abilities.FirstEdition.CaptainNymRebelAbiliity
    {
        public override void ActivateAbility()
        {
            base.ActivateAbility();
            HostShip.AfterGotNumberOfDefenceDice += CheckBombObstruction;
        }

        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
            HostShip.AfterGotNumberOfDefenceDice -= CheckBombObstruction;
        }

        private void CheckBombObstruction(ref int count)
        {
            if (Combat.ShotInfo.IsObstructedByBombToken)
            {
                Messages.ShowInfo("Captain Nym: +1 defense die");
                count++;
            }
        }

        protected override bool CanUseAbility()
        {
            return HostShip.State.Charges > 0;
        }

        protected override void MarkAbilityAsUsed()
        {
            HostShip.SpendCharge();
        }
    }
}