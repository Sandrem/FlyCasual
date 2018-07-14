using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;
using SubPhases;
using BoardTools;
using Abilities;
using System.Linq;
using Arcs;

namespace Ship
{
    namespace LancerClassPursuitCraft
    {
        public class KetsuOnyo : LancerClassPursuitCraft
        {
            public KetsuOnyo() : base()
            {
                PilotName = "Ketsu Onyo";
                PilotSkill = 7;
                Cost = 38;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new KetsuOnyoPilotAbility());
            }
        }
    }
}


namespace Abilities
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
            SelectTargetForAbility(
                CheckAssignTractorBeam,
                FilterTargetsOfAbility,
                GetAiPriorityOfTarget,
                HostShip.Owner.PlayerNo,
                true,
                null,
                HostShip.PilotName,
                "Choose a ship inside your primary and mobile firing arcs to assign 1 Tractor Beam token to it.",
                HostShip.ImageUrl
            );
        }

        private bool FilterTargetsOfAbility(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapon);

            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy, TargetTypes.OtherFriendly })
                && FilterTargetsByRange(ship, 1, 1)
                && shotInfo.InArcByType(ArcTypes.Mobile)
                && shotInfo.InPrimaryArc;
        }

        private int GetAiPriorityOfTarget(GenericShip ship)
        {
            return 50;
        }

        private void CheckAssignTractorBeam()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            ShotInfo shotInfo = new ShotInfo(HostShip, TargetShip, HostShip.PrimaryWeapon);
            if (shotInfo.InArcByType(ArcTypes.Mobile) && shotInfo.InPrimaryArc && shotInfo.Range == 1)
            {
                Messages.ShowError(HostShip.PilotName + " assigns Tractor Beam Token\nto " + TargetShip.PilotName);
                Tokens.TractorBeamToken token = new Tokens.TractorBeamToken(TargetShip, HostShip.Owner);
                TargetShip.Tokens.AssignToken(token, Triggers.FinishTrigger);
            }
            else
            {
                if (!shotInfo.InArcByType(ArcTypes.Mobile) || !shotInfo.InPrimaryArc) 
                {
                    Messages.ShowError ("Target is not inside Mobile Arc and Primary Arc");
                } 
                else if (shotInfo.Range > 1) 
                {
                    Messages.ShowError ("Target is outside range 1");
                }
            }
        }

    }
}
