using Abilities;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    namespace AWing
    {
        public class JakeFarrell : AWing
        {
            public JakeFarrell() : base()
            {
                PilotName = "Jake Farrell";
                PilotSkill = 7;
                Cost = 24;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new JakeFarrellAbility());
            }
        }
    }
}

namespace Abilities
{
    public class JakeFarrellAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += RegisterJakeFarrellAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= RegisterJakeFarrellAbility;
        }

        private void RegisterJakeFarrellAbility(Ship.GenericShip ship, Type tokenType)
        {
            if (tokenType == typeof(Tokens.FocusToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, FreeRepositionAction);
            }
        }

        private void FreeRepositionAction(object sender, EventArgs e)
        {
            GenericShip originalSelectedShip = Selection.ThisShip;
            Selection.ThisShip = HostShip;
            List<ActionsList.GenericAction> actions = new List<ActionsList.GenericAction>() { new ActionsList.BoostAction(), new ActionsList.BarrelRollAction() };
            HostShip.AskPerformFreeAction(
                actions,
                delegate ()
                {
                    Selection.ThisShip = originalSelectedShip;
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}