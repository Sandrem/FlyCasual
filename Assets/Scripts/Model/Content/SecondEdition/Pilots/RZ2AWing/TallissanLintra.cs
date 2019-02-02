using ActionsList;
using Arcs;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections;
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
                PilotInfo = new PilotCardInfo(
                    "Tallissan Lintra",
                    5,
                    35,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TallissanLintraAbility),
                    charges: 1,
                    regensCharges: true,
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Talent } //,
                    //seImageNumber: 19
                );

                ModelInfo.SkinName = "Blue";

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/72cb6c4e50b0ad24af0bb84ce0aa53f0.png";
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

            if (IsDifferentPlayer && InTaliArc)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskIncreaseDefense);
            }
        }

        protected void AskIncreaseDefense(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, IncreaseDefense, null, null, false);
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