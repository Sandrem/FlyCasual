using Abilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;

namespace Ship
{
    namespace TIEReaper
    {
        public class MajorVermeil : TIEReaper
        {
            public MajorVermeil() : base()
            {
                PilotName = "Major Vermeil";
                PilotSkill = 6;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new MajorVermeilAbility());
            }
        }
    }
}

namespace Abilities
{
    public class MajorVermeilAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += AddMajorVermeilModifierEffect;
        }
                
        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= AddMajorVermeilModifierEffect;
        }

        private void AddMajorVermeilModifierEffect(Ship.GenericShip ship)
        {
            if(Combat.Attacker.ShipId == ship.ShipId 
                && !(Combat.Defender.Tokens.HasToken<EvadeToken>() || Combat.Defender.Tokens.HasToken<FocusToken>()) 
                && (Combat.DiceRollAttack.Focuses > 0 || Combat.DiceRollAttack.Blanks > 0))
            {                
                ship.AddAvailableActionEffect(new MajorVermeilAction
                {
                    ImageUrl = HostShip.ImageUrl,
                    Host = HostShip
                });
            }
        }

        private class MajorVermeilAction : ActionsList.GenericAction
        {
            public MajorVermeilAction()
            {
                Name = EffectName = "Major Vermeil's ability";

                IsTurnsOneFocusIntoSuccess = true;                
            }

            public override void ActionEffect(System.Action callBack)
            {
                if (Combat.CurrentDiceRoll.Blanks > 0)
                {
                    Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
                }
                else if (Combat.CurrentDiceRoll.Focuses > 0)
                {
                    Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                }
                callBack();
            }

            public override bool IsActionEffectAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Attack) result = true;
                return result;
            }

            public override int GetActionEffectPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack)
                {
                    int attackBlanks = Combat.DiceRollAttack.Blanks;
                    if (attackBlanks > 0)
                    {
                        if ((attackBlanks == 1) && (Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0))
                        {
                            result = 100;
                        }
                        else
                        {
                            result = 55;
                        }
                    }
                }

                return result;
            }
        }
    }
}