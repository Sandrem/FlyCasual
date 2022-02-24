using Arcs;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEWiWhisperModifiedInterceptor
    {
        public class KyloRenWhisper : TIEWiWhisperModifiedInterceptor
        {
            public KyloRenWhisper() : base()
            {
                PilotInfo = new PilotCardInfo
                (
                    "Kylo Ren",
                    5,
                    63,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KyloRenWhisperPilotAbility),
                    force: 3,
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.ForcePower, UpgradeType.Talent }
                );

                PilotNameCanonical = "kyloren-tiewiwhispermodifiedinterceptor";

                ImageUrl = "https://i.imgur.com/1PGGda3.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KyloRenWhisperPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnDamageCardSeverityIsCheckedGlobal += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDamageCardSeverityIsCheckedGlobal -= CheckConditions;
        }

        private void CheckConditions(GenericShip ship)
        {
            if (!Combat.CurrentCriticalHitCard.IsFaceup
                && !Tools.IsSameTeam(HostShip, ship)
                && HostShip.State.Force > 0
                && HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Bullseye))
            {
                RegisterAbilityTrigger(TriggerTypes.OnDamageCardSeverityIsChecked, AskUseKyloRenAbility);
            }
        }

        private void AskUseKyloRenAbility(object sender, System.EventArgs e)
        {
            GenericShip previousShip = Selection.ActiveShip;
            Selection.ActiveShip = sender as GenericShip;

            AskToUseAbility
            (
                descriptionShort: HostShip.PilotInfo.PilotName,
                IsShouldUseAbility,
                UseAbility,
                delegate
                {
                    Selection.ActiveShip = previousShip;
                    DecisionSubPhase.ConfirmDecision();
                },
                descriptionLong: "Do you want to spend 1 Force to deal that facedown damage card as faceup instead?",
                imageHolder: HostShip
            );
        }

        private bool IsShouldUseAbility()
        {
            return true;
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Combat.CurrentCriticalHitCard.IsFaceup = true;
            HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
        }
    }
}
