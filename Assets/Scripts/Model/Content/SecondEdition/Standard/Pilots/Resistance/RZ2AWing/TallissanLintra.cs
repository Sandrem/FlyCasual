using Arcs;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class TallissanLintra : RZ2AWing
        {
            public TallissanLintra() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Tallissan Lintra",
                    "Deadly Approach",
                    Faction.Resistance,
                    5,
                    4,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TallissanLintraAbility),
                    charges: 1,
                    regensCharges: 1,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    },
                    skinName: "Blue"
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TallissanLintraAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += CheckPilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= CheckPilotAbility;
        }

        protected virtual void CheckPilotAbility()
        {
            bool IsDifferentPlayer = (HostShip.Owner.PlayerNo != Combat.Attacker.Owner.PlayerNo);
            bool InTaliArc = HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, ArcType.Bullseye);

            if (IsDifferentPlayer && InTaliArc && HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskIncreaseDefense);
            }
        }

        protected void AskIncreaseDefense(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                IncreaseDefense,
                descriptionLong: "Do you want to spend 1 Charge to allow defender to roll 1 additional die?",
                imageHolder: HostShip
            );
        }

        protected virtual void IncreaseDefense(object sender, System.EventArgs e)
        {
            HostShip.State.Charges--;
            Combat.Defender.AfterGotNumberOfDefenceDice += IncreaseNumberOfDefenseDie;
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void IncreaseNumberOfDefenseDie(ref int diceCount)
        {
            diceCount++;
            Combat.Defender.AfterGotNumberOfDefenceDice -= IncreaseNumberOfDefenseDie;
        }
    }
}