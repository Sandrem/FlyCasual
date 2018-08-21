using Abilities;
using RuleSets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;

namespace Ship
{
    namespace TIEReaper
    {
        public class MajorVermeil : TIEReaper, ISecondEditionPilot
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

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 49;

                PilotAbilities.RemoveAll(ability => ability is Abilities.MajorVermeilAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.MajorVermeilAbilitySE());
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
            HostShip.OnGenerateDiceModifications += AddMajorVermeilModifierEffect;
        }
                
        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddMajorVermeilModifierEffect;
        }

        protected virtual void AddMajorVermeilModifierEffect(Ship.GenericShip ship)
        {
            if(Combat.Attacker.ShipId == ship.ShipId 
                && !(Combat.Defender.Tokens.HasToken<EvadeToken>() || Combat.Defender.Tokens.HasToken<FocusToken>()) 
                && (Combat.DiceRollAttack.Focuses > 0 || Combat.DiceRollAttack.Blanks > 0))
            {                
                ship.AddAvailableDiceModification(new MajorVermeilAction
                {
                    ImageUrl = HostShip.ImageUrl,
                    Host = HostShip
                });
            }
        }

        protected class MajorVermeilAction : ActionsList.GenericAction
        {
            public MajorVermeilAction()
            {
                Name = DiceModificationName = "Major Vermeil's ability";

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

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Attack) result = true;
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack)
                {
                    int attackBlanks = Combat.DiceRollAttack.Blanks;
                    if (attackBlanks > 0)
                    {
                        if ((attackBlanks == 1) && (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0))
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

namespace Abilities.SecondEdition
{
    public class MajorVermeilAbilitySE : MajorVermeilAbility
    {
        protected override void AddMajorVermeilModifierEffect(Ship.GenericShip ship)
        {
            if (Combat.Attacker.ShipId == ship.ShipId
                && !Combat.Defender.Tokens.HasGreenTokens()
                && (Combat.DiceRollAttack.Focuses > 0 || Combat.DiceRollAttack.Blanks > 0))
            {
                ship.AddAvailableDiceModification(new MajorVermeilAction
                {
                    ImageUrl = HostShip.ImageUrl,
                    Host = HostShip
                });
            }
        }
    }
}