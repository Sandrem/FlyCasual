using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using BoardTools;
using RuleSets;
using SubPhases;
using Ship.YT2400;

namespace Ship
{
    namespace TIEAdvPrototype
    {
        public class TheInquisitor : TIEAdvPrototype, ISecondEditionPilot
        {
            public TheInquisitor() : base()
            {
                PilotName = "The Inquisitor";
                PilotSkill = 8;
                Cost = 25;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.TheInquisitorAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotName = "Grand Inquisitor";
                PilotSkill = 5;
                Cost = 58;
                MaxForce = 2;

                PilotAbilities.RemoveAll(ability => ability is Abilities.TheInquisitorAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.GrandInquisitorAbilitySE());
            }
        }
    }
}

namespace Abilities
{
    public class TheInquisitorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            ShotInfo.OnRangeIsMeasured += SetRangeToOne;
        }

        public override void DeactivateAbility()
        {
            ShotInfo.OnRangeIsMeasured -= SetRangeToOne;
        }

        private void SetRangeToOne(GenericShip thisShip, GenericShip anotherShip, IShipWeapon chosenWeapon, ref int range)
        {
            if (thisShip.ShipId == HostShip.ShipId)
            {
                if ((range <= 3) && (chosenWeapon.GetType() == typeof(PrimaryWeaponClass)))
                {
                    range = 1;
                }
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // Temporary implementation. If in the future other abilities manipulate range bonus this will need to change.
    public class GrandInquisitorAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterInquisitorAttackAbility;
            HostShip.OnAttackStartAsDefender += RegisterInquisitorDefenseAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterInquisitorAttackAbility;
            HostShip.OnAttackStartAsDefender -= RegisterInquisitorDefenseAbility;
        }

        private void RegisterInquisitorAttackAbility()
        {
            if (HostShip.Force < 1)
                return;

            if (Combat.ShotInfo.Range == 1)
                return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, delegate
            {
                AskToUseAbility(AlwaysUseByDefault, UseInquisitorAttackAbility);
            });
        }

        private void RegisterInquisitorDefenseAbility()
        {
            if (HostShip.Force < 1)
                return;

            if (Combat.ShotInfo.Range > 1)
                return;

            // What a hack batman.
            if (Combat.Attacker is YT2400)
                return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, delegate
            {
                AskToUseAbility(AlwaysUseByDefault, UseInquisitorDefenseAbility);
            });
        }

        private void UseInquisitorAttackAbility(object sender, EventArgs e)
        {
            HostShip.Force--;
            HostShip.AfterGotNumberOfAttackDice += IncreaseAttackDice;
            DecisionSubPhase.ConfirmDecision();
        }

        private void UseInquisitorDefenseAbility(object sender, EventArgs e)
        {
            HostShip.Force--;
            Combat.Attacker.AfterGotNumberOfAttackDice += DecreaseAttackDice;
            DecisionSubPhase.ConfirmDecision();
        }

        private void IncreaseAttackDice(ref int result)
        {
            result++;
            HostShip.AfterGotNumberOfAttackDice -= IncreaseAttackDice;
        }

        private void DecreaseAttackDice(ref int result)
        {
            result--;
            Combat.Attacker.AfterGotNumberOfAttackDice -= DecreaseAttackDice;
        }
    }
}