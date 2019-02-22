using Ship;
using BoardTools;
using Arcs;
using Upgrade;
using System.Collections.Generic;
using SubPhases;

namespace Ship
{
    namespace FirstEdition.LancerClassPursuitCraft
    {
        public class KetsuOnyo : LancerClassPursuitCraft
        {
            public KetsuOnyo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ketsu Onyo",
                    7,
                    38,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.KetsuOnyoPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class KetsuOnyoPilotAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += TryRegisterKetsuOnyoPilotAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= TryRegisterKetsuOnyoPilotAbility;
        }

        private void TryRegisterKetsuOnyoPilotAbility()
        {
            if (TargetsForAbilityExist(FilterTargetsOfAbility))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskSelectShip);
            }
        }

        private void AskSelectShip(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            SelectTargetForAbility(
                CheckAssignTractorBeam,
                FilterTargetsOfAbility,
                GetAiPriorityOfTarget,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a ship inside your primary and mobile firing arcs to assign 1 Tractor Beam token to it.",
                HostShip
            );
        }

        private bool FilterTargetsOfAbility(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapons);

            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy, TargetTypes.OtherFriendly })
                && FilterTargetsByRange(ship, 0, 1)
                && shotInfo.InArcByType(ArcType.SingleTurret)
                && shotInfo.InPrimaryArc;
        }

        private int GetAiPriorityOfTarget(GenericShip ship)
        {
            return 50;
        }

        private void CheckAssignTractorBeam()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            ShotInfo shotInfo = new ShotInfo(HostShip, TargetShip, HostShip.PrimaryWeapons);
            if (shotInfo.InArcByType(ArcType.SingleTurret) && shotInfo.InPrimaryArc && shotInfo.Range == 1)
            {
                Messages.ShowError(HostShip.PilotInfo.PilotName + " assigns Tractor Beam Token\nto " + TargetShip.PilotInfo.PilotName);
                Tokens.TractorBeamToken token = new Tokens.TractorBeamToken(TargetShip, HostShip.Owner);
                TargetShip.Tokens.AssignToken(token, Triggers.FinishTrigger);
            }
            else
            {
                if (!shotInfo.InArcByType(ArcType.SingleTurret) || !shotInfo.InPrimaryArc)
                {
                    Messages.ShowError("Target is not inside Mobile Arc and Primary Arc");
                }
                else if (shotInfo.Range > 1)
                {
                    Messages.ShowError("Target is outside range 1");
                }
            }
        }

    }
}