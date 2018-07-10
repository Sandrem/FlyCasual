using RuleSets;
using Ship;
using System.Collections.Generic;
using System.Linq;

namespace Ship
{
    namespace Firespray31
    {
        public class BobaFettSV : Firespray31, ISecondEditionPilot
        {
            public BobaFettSV() : base()
            {
                PilotName = "Boba Fett";
                PilotSkill = 8;
                Cost = 39;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                faction = Faction.Scum;

                SkinName = "Boba Fett";

                PilotAbilities.Add(new Abilities.BobaFettSVAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 80;
            }
        }
    }
}

namespace Abilities
{
    public class BobaFettSVAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Reroll,
                GetNumberOfEnemyShipsAtRange1,
                new List<DieSide>() { DieSide.Blank, DieSide.Focus, DieSide.Success, DieSide.Crit }
            );
        }

        private bool IsDiceModificationAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Attack) || (Combat.AttackStep == CombatStep.Defence))
            {
                if (GetNumberOfEnemyShipsAtRange1() > 0) result = true;
            }
            return result;
        }

        private int GetNumberOfEnemyShipsAtRange1()
        {
            return BoardTools.Board.GetShipsAtRange(HostShip, new UnityEngine.Vector2(0, 1), Team.Type.Enemy).Count;
        }

        private int GetDiceModificationPriority()
        {
            return 90;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}