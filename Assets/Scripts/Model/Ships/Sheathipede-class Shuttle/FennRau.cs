using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Abilities;

namespace Ship
{
    namespace SheathipedeShuttle
    {
        public class FennRau : SheathipedeShuttle
        {
            public FennRau() : base()
            {
                PilotName = "Fenn Rau";
                PilotSkill = 9;
                Cost = 20;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new FennRauRebelAbility());
            }
        }
    }
}

namespace Abilities
{

    public class FennRauRebelAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnCombatActivationGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnCombatActivationGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip activatedShip)
        {
            if (activatedShip.Owner.PlayerNo == HostShip.Owner.PlayerNo) return;

            if (HostShip.Tokens.HasToken(typeof(StressToken))) return;

            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(HostShip, activatedShip);
            if (!shotInfo.InArc || shotInfo.Range > 3) return;

            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, AskAbility);
        }

        private void AskAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, UseAbility);
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                HostShip.Tokens.AssignToken(
                    new StressToken(HostShip),
                    AssignConditionToActivatedShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman("Fenn Rau: Cannot use ability - already has stress");
                Triggers.FinishTrigger();
            }
        }

        private void AssignConditionToActivatedShip()
        {
            DecisionSubPhase.ConfirmDecision();
        }
    }
}
