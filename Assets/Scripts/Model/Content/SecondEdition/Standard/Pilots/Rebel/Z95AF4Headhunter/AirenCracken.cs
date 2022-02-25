using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class AirenCracken : Z95AF4Headhunter
        {
            public AirenCracken() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Airen Cracken",
                    "Intelligence Chief",
                    Faction.Rebel,
                    5,
                    3,
                    9,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.AirenCrackenAbiliity),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                        UpgradeType.Modification
                    },
                    seImageNumber: 27
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AirenCrackenAbiliity : GenericAbility
    {
        // After you perform an attack, you may choose another friendly ship at
        // range 1. That ship may perform 1 free action
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += TryRegisterAirenCrackenAbiliity;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= TryRegisterAirenCrackenAbiliity;
        }

        private void TryRegisterAirenCrackenAbiliity(GenericShip ship)
        {
            if (BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(1, 1), Team.Type.Friendly).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, this.AskSelectShip);
            }
        }

        private void AskSelectShip(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                PerformFreeAction,
                FilterAbilityTargets,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose another ship, that ship may perform free action",
                HostShip
            );
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 1);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            result += NeedTokenPriority(ship);
            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        private int NeedTokenPriority(GenericShip ship)
        {
            if (!ship.Tokens.HasToken(typeof(Tokens.FocusToken))) return 100;
            if (ship.ActionBar.HasAction(typeof(ActionsList.EvadeAction)) && !ship.Tokens.HasToken(typeof(Tokens.EvadeToken))) return 50;
            if (ship.ActionBar.HasAction(typeof(ActionsList.TargetLockAction)) && !ship.Tokens.HasToken(typeof(Tokens.BlueTargetLockToken), '*')) return 50;
            return 0;
        }

        private void PerformFreeAction()
        {
            Selection.ThisShip = TargetShip;

            TargetShip.AskPerformFreeAction(
                TargetShip.GetAvailableActionsAsRed(),
                delegate {
                    Selection.ThisShip = HostShip;
                    SelectShipSubPhase.FinishSelection();
                },
                HostShip.PilotInfo.PilotName,
                "You may perform an action, treating it as red",
                HostShip
            );
        }
    }
}
