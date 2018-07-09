using Movement;
using ActionsList;
using RuleSets;
using Abilities.SecondEdition;
using Ship;
using System;
using System.Collections.Generic;

namespace Ship
{
    namespace AWing
    {
        public class AWing : GenericShip, ISecondEditionShip
        {

            public AWing() : base()
            {
                Type = "A-Wing";

                IconicPilots.Add(Faction.Rebel, typeof(TychoCelchu));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/0/0c/MR_A-WING.png";

                Firepower = 2;
                Agility = 3;
                MaxHull = 2;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);

                PrintedActions.Add(new TargetLockAction());
                PrintedActions.Add(new EvadeAction());
                PrintedActions.Add(new BoostAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.AWingTable();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "Red";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 2;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Normal);
                Maneuvers.Add("1.L.B", MovementComplexity.None);
                Maneuvers.Add("1.F.S", MovementComplexity.None);
                Maneuvers.Add("1.R.B", MovementComplexity.None);
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("1.F.R", MovementComplexity.None);
                Maneuvers.Add("2.L.T", MovementComplexity.Easy);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Easy);
                Maneuvers.Add("2.F.R", MovementComplexity.None);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.F.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Easy);
                Maneuvers.Add("4.F.R", MovementComplexity.None);
                Maneuvers.Add("5.F.S", MovementComplexity.Easy);
                Maneuvers.Add("5.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                PrintedActions.Add(new BarrelRollAction());

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Configuration);

                Maneuvers.Add("3.L.R", MovementComplexity.Complex);
                Maneuvers.Add("3.R.R", MovementComplexity.Complex);
                Maneuvers.Remove("3.F.R");

                ShipAbilities.Add(new VectoredThrusters());

                IconicPilots[Faction.Rebel] = typeof(PrototypePilot);
            }
        }
    }
}


namespace Abilities.SecondEdition
{
    //After you perform an action, you may perform a red boost action.
    public class VectoredThrusters : GenericAbility
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
            if (!(action is BoostAction))
            {
                HostShip.OnActionDecisionSubphaseEnd += PerformBoostAction;
            }
        }

        private void PerformBoostAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= PerformBoostAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, AskPerformBoostAction);
        }

        private void AskPerformBoostAction(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("Vectored Thrusters: you may perform a red boost action");

            HostShip.AskPerformFreeAction(
                new BoostAction() { IsRed = true },
                Triggers.FinishTrigger
            );
        }
    }
}
