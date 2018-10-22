using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;
using Ship;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class TIEInterceptor : GenericShip, TIE, ISecondEditionShip
        {

            public TIEInterceptor() : base()
            {
                Type = FullType = "TIE Interceptor";
                IconicPilots.Add(Faction.Imperial, typeof(SoontirFel));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/8/8e/MI_TIE-INTERCEPTOR.png";

                Firepower = 3;
                Agility = 3;
                MaxHull = 3;
                MaxShields = 0;

                ActionBar.AddPrintedAction(new EvadeAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());
                ActionBar.AddPrintedAction(new BoostAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEInterceptorTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "Gray";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 3;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Normal);
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.T", MovementComplexity.Easy);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Easy);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.F.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Easy);
                Maneuvers.Add("5.F.S", MovementComplexity.Normal);
                Maneuvers.Add("5.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                Maneuvers.Remove("3.F.R");
                Maneuvers.Add("3.L.R", MovementComplexity.Complex);
                Maneuvers.Add("3.R.R", MovementComplexity.Complex);

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Modification);

                ShipAbilities.Add(new Abilities.SecondEdition.AutoThrusters());
            }

        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an action, you may perform a red boost / red barrel roll action.
    public class AutoThrusters : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            HostShip.OnActionDecisionSubphaseEnd += PerformRepositionAction;
        }

        private void PerformRepositionAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= PerformRepositionAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, AskPerformPerositionAction);
        }

        private void AskPerformPerositionAction(object sender, System.EventArgs e)
        {
            if (!HostShip.IsStressed)
            {
                Messages.ShowInfoToHuman("AutoThrusters: you may perform a red action");

                HostShip.AskPerformFreeAction(
                    new List<GenericAction>()
                    {
                        new BoostAction() { IsRed = true },
                        new BarrelRollAction() { IsRed = true }
                    },
                    Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
