using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;

namespace Ship
{
    namespace T70XWing
    {
        public class PoeDameron : T70XWing
        {
            public PoeDameron() : base()
            {
                PilotName = "Poe Dameron";
                PilotSkill = 8;
                Cost = 31;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Black One";

                PilotAbilities.Add(new PoeDameronAbility());
            }
        }
    }
}

namespace Abilities
{
    public class PoeDameronAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += AddPoeDameronPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= AddPoeDameronPilotAbility;
        }

        private void AddPoeDameronPilotAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new PoeDameronAction() { Host = HostShip });
        }

        private class PoeDameronAction : ActionsList.GenericAction
        {
            public PoeDameronAction()
            {
                Name = EffectName = "Poe Dameron's ability";

                IsTurnsOneFocusIntoSuccess = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                if (Host.HasToken(typeof(Tokens.FocusToken)))
                {
                    Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                }
                else
                {
                    Messages.ShowErrorToHuman("Cannot use ability - no Focus token");
                }
                
                callBack();
            }

            public override bool IsActionEffectAvailable()
            {
                bool result = false;
                if ((Host.HasToken(typeof(Tokens.FocusToken))) && (Combat.CurrentDiceRoll.Focuses > 0)) result = true;
                return result;
            }

            public override int GetActionEffectPriority()
            {
                int result = 0;

                if (Host.HasToken(typeof(Tokens.FocusToken)))
                {
                    if (Combat.CurrentDiceRoll.Focuses > 0) result = 100;
                }

                return result;
            }
        }

    }
}
