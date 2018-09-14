using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuleSets;
using Ship;
using Upgrade;
using SubPhases;

namespace Ship
{
    namespace YT1300
    {
        public class Chewbacca : YT1300, ISecondEditionPilot
        {
            public Chewbacca() : base()
            {
                PilotName = "Chewbacca";
                PilotSkill = 5;
                Cost = 42;

                IsUnique = true;

                Firepower = 3;
                MaxHull = 8;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.ChewbaccaAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 84;
                MaxCharges = 1;
                RegensCharges = true;

                PilotAbilities.RemoveAll(ability => ability is Abilities.ChewbaccaAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.ChewbaccaAbilitySE());

                SEImageNumber = 71;
            }
        }
    }
}

namespace Abilities
{
    public class ChewbaccaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckFaceupCrit += FlipCrits;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckFaceupCrit -= FlipCrits;
        }

        private void FlipCrits(ref bool result)
        {
            if (result == true)
            {
                Messages.ShowInfo("Chewbacca: Crit is flipped facedown");
                Sounds.PlayShipSound("Chewbacca");
                result = false;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ChewbaccaAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDamageCardIsDealt += RegisterChewbaccaTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDamageCardIsDealt -= RegisterChewbaccaTrigger;
        }

        private void RegisterChewbaccaTrigger(GenericShip ship)
        {
            if (Combat.CurrentCriticalHitCard.IsFaceup && HostShip.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDamageCardIsDealt, AskUseChewbaccaAbility);
            }
        }

        private void AskUseChewbaccaAbility(object sender, System.EventArgs e)
        {
            GenericShip previousShip = Selection.ActiveShip;
            Selection.ActiveShip = sender as GenericShip;

            AskToUseAbility(
                IsShouldUseAbility,
                UseAbility,
                delegate
                {
                    Selection.ActiveShip = previousShip;
                    SubPhases.DecisionSubPhase.ConfirmDecision();
                });
        }

        private bool IsShouldUseAbility()
        {
            return true;
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Sounds.PlayShipSound("Chewbacca");
            HostShip.Charges--;
            Combat.CurrentCriticalHitCard.IsFaceup = false;
            DecisionSubPhase.ConfirmDecision();
        }
    }
}