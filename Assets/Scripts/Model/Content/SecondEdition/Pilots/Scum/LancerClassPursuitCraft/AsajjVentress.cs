using Arcs;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LancerClassPursuitCraft
    {
        public class AsajjVentress : LancerClassPursuitCraft
        {
            public AsajjVentress() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Asajj Ventress",
                    "Force of Her Own",
                    Faction.Scum,
                    4,
                    8,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.AsajjVentressPilotAbility),
                    force: 2,
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter,
                        Tags.DarkSide
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    seImageNumber: 219,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Asajj";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AsajjVentressPilotAbility : Abilities.FirstEdition.AsajjVentressPilotAbility
    {

        protected override void TryRegisterAsajjVentressPilotAbility()
        {
            if (TargetsForAbilityExist(FilterTargetsOfAbility) && HostShip.State.Force > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskSelectShip);
            }
        }

        protected override void AskSelectShip(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            SelectTargetForAbility(
                CheckAssignStress,
                FilterTargetsOfAbility,
                GetAiPriorityOfTarget,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a ship inside your mobile firing arc to assign Stress token to it",
                HostShip
            );
        }

        private void CheckAssignStress()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            AsajjVentressAbilityDecisionSubPhaseSE subphase = (AsajjVentressAbilityDecisionSubPhaseSE)
                Phases.StartTemporarySubPhaseNew(
                "Choose effect of " + HostShip.PilotInfo.PilotName + "' ability.",
                typeof(AsajjVentressAbilityDecisionSubPhaseSE),
                Triggers.FinishTrigger
            );

            Selection.ThisShip = TargetShip;
            Selection.ActiveShip = HostShip;
            subphase.SourceUpgrade = HostUpgrade;
            subphase.Start();
        }
    }
}

namespace Abilities.FirstEdition
{
    public class AsajjVentressPilotAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += TryRegisterAsajjVentressPilotAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= TryRegisterAsajjVentressPilotAbility;
        }

        protected virtual void TryRegisterAsajjVentressPilotAbility()
        {
            if (TargetsForAbilityExist(FilterTargetsOfAbility))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskSelectShip);
            }
        }

        protected virtual void AskSelectShip(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            SelectTargetForAbility(
                CheckAssignStress,
                FilterTargetsOfAbility,
                GetAiPriorityOfTarget,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a ship inside your mobile firing arc to assign Stress token to it",
                HostShip
            );
        }

        protected bool FilterTargetsOfAbility(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 2) && FilterTargetInMobileFiringArc(ship);
        }

        protected int GetAiPriorityOfTarget(GenericShip ship)
        {
            int priority = 50;

            priority += (ship.Tokens.CountTokensByType(typeof(StressToken)) * 25);
            priority += (ship.State.Agility * 5);

            if (ship.CallCheckCanPerformActionsWhileStressed() && ship.CanPerformRedManeuverWhileStressed()) priority = 10;

            return priority;
        }

        private bool FilterTargetInMobileFiringArc(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapons);
            return shotInfo.InArcByType(ArcType.SingleTurret);
        }

        private void CheckAssignStress()
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, TargetShip, HostShip.PrimaryWeapons);
            if (shotInfo.InArcByType(ArcType.SingleTurret) && shotInfo.Range >= 1 && shotInfo.Range <= 2)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " assigns a Stress token\nto " + TargetShip.PilotInfo.PilotName);
                TargetShip.Tokens.AssignToken(typeof(StressToken), SelectShipSubPhase.FinishSelection);
            }
            else
            {
                if (!shotInfo.InArcByType(ArcType.SingleTurret)) Messages.ShowError("The target is not inside " + HostShip.PilotInfo.PilotName + "'s Mobile Arc");
                else if (shotInfo.Range >= 3) Messages.ShowError("The target is outside range 2");
            }
        }

    }
}

namespace SubPhases
{
    public class AsajjVentressAbilityDecisionSubPhaseSE : RemoveGreenTokenDecisionSubPhase
    {
        public GenericUpgrade SourceUpgrade;

        public override void PrepareCustomDecisions()
        {
            DescriptionShort = "Asajj Ventress";
            DescriptionLong = Selection.ThisShip.ShipId + ": " + "Select the effect of Asajj Ventress' ability.";
            ImageSource = SourceUpgrade;

            DecisionOwner = Selection.ThisShip.Owner;
            DefaultDecisionName = "Recieve a stress token.";

            AddDecision("Recieve a stress token.", RecieveStress);
        }

        private void RecieveStress(object sender, System.EventArgs e)
        {
            Selection.ActiveShip.State.SpendForce(
                1,
                delegate { Selection.ThisShip.Tokens.AssignToken(typeof(StressToken), DecisionSubPhase.ConfirmDecision); }
            );
        }
    }
}