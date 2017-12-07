using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using SubPhases;

namespace Ship
{
    namespace YWing
    {
        public class DutchVander : YWing
        {
            public DutchVander() : base()
            {
                PilotName = "\"Dutch\" Vander";
                PilotSkill = 6;
                Cost = 23;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                faction = Faction.Rebel;

                PilotAbilities.Add(new AbilitiesNamespace.DutchVanderAbility());
            }
        }
    }
}

namespace AbilitiesNamespace
{
    public class DutchVanderAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            HostShip.OnTokenIsAssigned += DutchVanderPilotAbility;
        }

        private void DutchVanderPilotAbility(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(Tokens.BlueTargetLockToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, StartSubphaseForDutchVanderPilotAbility);
            }
        }

        private void StartSubphaseForDutchVanderPilotAbility(object sender, System.EventArgs e)
        {
            Selection.ThisShip = HostShip;
            if (HostShip.Owner.Ships.Count > 1)
            {
                SelectTargetForAbility(
                    GrantFreeTargetLock,
                    new List<TargetTypes>() { TargetTypes.OtherFriendly },
                    new Vector2(1, 2)
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void GrantFreeTargetLock()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, StartSubphaseForTargetLock);

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, SelectShipSubPhase.FinishSelection);
        }

        private void StartSubphaseForTargetLock(object sender, System.EventArgs e)
        {
            Selection.ThisShip = TargetShip;

            Phases.StartTemporarySubPhaseOld(
                "Select target for Target Lock",
                typeof(FreeSelectTargetLockSubPhase),
                SelectShipSubPhase.FinishSelection
            );
        }

        private void TrySelectTargetLock()
        {
            Actions.AssignTargetLockToPair(
                Selection.ThisShip,
                TargetShip,
                SelectShipSubPhase.FinishSelection,
                (Phases.CurrentSubPhase as SelectShipSubPhase).RevertSubPhase
            );
        }
    }
}

namespace SubPhases
{
    public class FreeSelectTargetLockSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            targetsAllowed.Add(TargetTypes.Enemy);
            finishAction = TrySelectTargetLock;

            UI.ShowSkipButton();
        }

        private void TrySelectTargetLock()
        {
            Actions.AssignTargetLockToPair(
                Selection.ThisShip,
                TargetShip,
                CallBack,
                RevertSubPhase
            );
        }

        public override void RevertSubPhase() { }

        public override void SkipButton()
        {
            CallBack();
        }

    }
}

