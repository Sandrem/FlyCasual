using UnityEngine;
using Ship;
using System.Collections;
using System.Collections.Generic;
using SubPhases;
using Abilities;
using Board;

namespace Ship
{
    namespace Z95
    {
        public class AirenCracken : Z95
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
            if (BoardManager.GetShipsAtRange(HostShip, new Vector2(1, 1), Team.Type.Friendly).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinishAsAttacker, AskSelectShip);
            }
        }

        private void AskSelectShip(object sender, System.EventArgs e)
        {
            SelectTargetForAbilityOld(PerformFreeAction, new List<TargetTypes>() { TargetTypes.OtherFriendly }, new Vector2(1, 1), null, true);
        }

        private void PerformFreeAction()
        {
            Selection.ThisShip = TargetShip;
            TargetShip.GenerateAvailableActionsList();
            TargetShip.AskPerformFreeAction(TargetShip.GetAvailableActionsList(),
                delegate{
                    Selection.ThisShip = HostShip;
                    SelectShipSubPhase.FinishSelection();
                });
        }
    }
}