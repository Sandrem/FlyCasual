﻿using UnityEngine;
using Ship;
using System.Collections;
using System.Collections.Generic;
using SubPhases;
using Abilities;
using BoardTools;
using System.Linq;
using RuleSets;

namespace Ship
{
    namespace Z95
    {
        public class AirenCracken : Z95, ISecondEditionPilot
        {
            public AirenCracken() : base()
            {
                PilotName = "Airen Cracken";
                PilotSkill = 8;
                Cost = 19;
                IsUnique = true;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
 
                PilotAbilities.Add(new AirenCrackenAbiliity());
                faction = Faction.Rebel;

                SkinName = "Red";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 36;

                PilotAbilities.RemoveAll(ability => ability is Abilities.AirenCrackenAbiliity);
                PilotAbilities.Add(new Abilities.SecondEdition.AirenCrackenAbilitySE());

                SEImageNumber = 27;
            }
        }
    }
}
 
namespace Abilities
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
                true,
                null,
                HostShip.PilotName,
                "Choose another ship.\nThat ship may perform free action.",
                HostShip.ImageUrl
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
            result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

            return result;
        }

        private int NeedTokenPriority(GenericShip ship)
        {
            if (!ship.Tokens.HasToken(typeof(Tokens.FocusToken))) return 100;
            if (ship.ActionBar.HasAction(typeof(ActionsList.EvadeAction)) && !ship.Tokens.HasToken(typeof(Tokens.EvadeToken))) return 50;
            if (ship.ActionBar.HasAction(typeof(ActionsList.TargetLockAction)) && !ship.Tokens.HasToken(typeof(Tokens.BlueTargetLockToken), '*')) return 50;
            return 0;
        }

        protected virtual void PerformFreeAction()
        {
            Selection.ThisShip = TargetShip;

            TargetShip.AskPerformFreeAction(
                TargetShip.GetAvailableActions(),
                delegate{
                    Selection.ThisShip = HostShip;
                    SelectShipSubPhase.FinishSelection();
                });
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AirenCrackenAbilitySE : AirenCrackenAbiliity
    {
        protected override void PerformFreeAction()
        {
            Selection.ThisShip = TargetShip;

            TargetShip.AskPerformFreeAction(
                TargetShip.GetAvailableActionsAsRed(),
                delegate {
                    Selection.ThisShip = HostShip;
                    SelectShipSubPhase.FinishSelection();
                });
        }
    }
}