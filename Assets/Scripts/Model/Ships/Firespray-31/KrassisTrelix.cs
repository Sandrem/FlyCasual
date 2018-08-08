using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using RuleSets;

namespace Ship
{
    namespace Firespray31
    {
        public class KrassisTrelix : Firespray31, ISecondEditionPilot
        {
            public KrassisTrelix() : base()
            {
                PilotName = "Krassis Trelix";
                PilotSkill = 5;
                Cost = 36;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.KrassisTrelixAbility());

                faction = Faction.Imperial;

                SkinName = "Krassis Trelix";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 70;

                faction = Faction.Scum;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.RemoveAll(a => a is Abilities.KrassisTrelixAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.KrassisTrelixAbilitySE());
            }
        }
    }
}

namespace Abilities
{
    public class KrassisTrelixAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += KrassisTrelixPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= KrassisTrelixPilotAbility;
        }

        public void KrassisTrelixPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new KrassisTrelixAction());
        }

        private class KrassisTrelixAction : ActionsList.GenericAction
        {
            public KrassisTrelixAction()
            {
                Name = DiceModificationName = "Krassis Trelix's ability";
                IsReroll = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = 1,
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Attack && (Combat.ChosenWeapon as Upgrade.GenericSecondaryWeapon) != null)
                {
                    result = true;
                }
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack && (Combat.ChosenWeapon as Upgrade.GenericSecondaryWeapon) != null)
                {
                    if (Combat.DiceRollAttack.Blanks > 0)
                    {
                        result = 90;
                    }
                    else if (Combat.DiceRollAttack.Focuses > 0 && Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                    {
                        result = 90;
                    }
                    else if (Combat.DiceRollAttack.Focuses > 0)
                    {
                        result = 30;
                    }
                }

                return result;
            }

        }
    }
}

namespace Abilities.SecondEdition
{
    public class KrassisTrelixAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            ToggleArcAbility(true);

            AddDiceModification(
                "Krassis Trelix",
                IsDiceModificationAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                1
            );
        }

        public override void DeactivateAbility()
        {
            ToggleArcAbility(false);

            RemoveDiceModification();
        }

        private void ToggleArcAbility(bool isActive)
        {
            HostShip.ArcInfo.GetArc<Arcs.ArcRear>().ShotPermissions.CanShootTorpedoes = isActive;
            HostShip.ArcInfo.GetArc<Arcs.ArcRear>().ShotPermissions.CanShootMissiles = isActive;
            HostShip.ArcInfo.GetArc<Arcs.ArcRear>().ShotPermissions.CanShootCannon = isActive;
        }

        private bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack && Combat.ChosenWeapon != HostShip.PrimaryWeapon;
        }

        private int GetAiPriority()
        {
            return 90;
        }
    }
}