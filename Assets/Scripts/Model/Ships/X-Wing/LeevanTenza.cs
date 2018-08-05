using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods.ModsList;
using Abilities;
using ActionsList;
using Tokens;
using SubPhases;
using RuleSets;

namespace Ship
{
    namespace XWing
    {
        public class LeevanTenza : XWing, ISecondEditionPilot
        {
            public LeevanTenza() : base()
            {
                PilotName = "Leevan Tenza";
                PilotSkill = 5;
                Cost = 25;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Partisan";

                PilotAbilities.Add(new LeevanTenzaAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 46;
            }
        }
    }
}

namespace Abilities
{
    public class LeevanTenzaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckLeevanTenzaAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckLeevanTenzaAbility;
        }

        private void CheckLeevanTenzaAbility(GenericAction action)
        {
            if (action is BoostAction || action is BarrelRollAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToUseLeevanTenzaAbility);
            }
        }

        private void AskToUseLeevanTenzaAbility(object sender, System.EventArgs e)
        {
            HostShip.AskPerformFreeAction(new EvadeAction() { IsRed = true }, DecisionSubPhase.ConfirmDecision);
        }
    }
}
