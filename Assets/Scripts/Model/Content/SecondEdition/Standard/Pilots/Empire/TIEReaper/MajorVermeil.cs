using Abilities.SecondEdition;
using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEReaper
    {
        public class MajorVermeil : TIEReaper
        {
            public MajorVermeil() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Major Vermeil",
                    "Veteran of Scarif",
                    Faction.Imperial,
                    4,
                    5,
                    16,
                    isLimited: true,
                    abilityType: typeof(MajorVermeilAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 113
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
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

        private void AddMajorVermeilModifierEffect(GenericShip ship)
        {
            if (Combat.Attacker.ShipId == ship.ShipId
                && !Combat.Defender.Tokens.HasGreenTokens
                && (Combat.DiceRollAttack.Focuses > 0 || Combat.DiceRollAttack.Blanks > 0))
            {
                ship.AddAvailableDiceModificationOwn(new MajorVermeilAction
                {
                    ImageUrl = HostShip.ImageUrl,
                    HostShip = HostShip
                });
            }
        }

        protected class MajorVermeilAction : GenericAction
        {
            public MajorVermeilAction()
            {
                Name = DiceModificationName = "Major Vermeil";

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