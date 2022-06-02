using Arcs;
using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Kylo Ren",
                    "Supreme Leader of the First Order",
                    Faction.FirstOrder,
                    5,
                    6,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KyloRenWhisperPilotAbility),
                    force: 3,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.ForcePower,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie,
                        Tags.DarkSide,
                        Tags.LightSide
                    }
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
