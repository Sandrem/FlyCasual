using Upgrade;
using System;
using Ship;
using Abilities;
using Tokens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradesList
{
    public class LeeboCrew : GenericUpgrade
    {
        public LeeboCrew() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Leebo";
            Cost = 2;
            isUnique = true;

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(31, 1));

            UpgradeAbilities.Add(new LeeboAbility());
        }

        public override bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class LeeboAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += LeeboAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= LeeboAction;
        }

        private void LeeboAction(GenericShip ship)
        {
            ActionsList.GenericAction action = new ActionsList.LeeboAction()
            {
                Host = this.HostShip
            };
            ship.AddAvailableAction(action);
        }
    }
}

namespace ActionsList
{

    // Perform a free Boost action. Then receive 1 ion token
    public class LeeboAction : GenericAction
    {
        public LeeboAction()
        {
            Name = DiceModificationName = "Leebo";
        }

        public override bool IsActionAvailable()
        {
            bool result = true;
            // can perform only one boost action per turn
            if (Host.IsAlreadyExecutedAction (typeof(BoostAction))) {
                result = false;
            }
            return result;
        }

        public override int GetDiceModificationPriority()
        {
            // low priority
            int result = 10;

            return result;
        }

        public override void ActionTake()
        {
            Phases.CurrentSubPhase.Pause();
            // should not be mandatory as it's checked by IsActionAvailable()
            if (!Selection.ThisShip.IsAlreadyExecutedAction(typeof(BoostAction)))
            {
                Phases.StartTemporarySubPhaseOld(
                    Name + ": Boost action",
                    typeof(SubPhases.BoostPlanningSubPhase),
                    AddIonToken
                );
            }
            else
            {
                // should never be there as it's already checked through IsActionAvailable
                Phases.CurrentSubPhase.Resume();
            }

        }

        private void AddIonToken()
        {
            // check if the free boost action has been performed
            // this may raise an issue where the Leebo free boost action is not recognized as a boost action. Therefore
            // a ship may be able to perform two boost actions per turn :/
            if (Host.IsAlreadyExecutedAction(typeof(LeeboAction)))
            {
                Messages.ShowInfoToHuman(Name + ": free boost performed, ion token received.");
                Host.Tokens.AssignToken(typeof(IonToken), Finish);
            }
            else
            {
                // if not, need to finish SubPhase
                Finish();
            }
        }

        private void Finish()
        {
            Phases.CurrentSubPhase.CallBack();
        }
    }
}
