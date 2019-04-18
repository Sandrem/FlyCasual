using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Firespray31
    {
        public class BobaFettScum : Firespray31
        {
            public BobaFettScum() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Boba Fett",
                    8,
                    39,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.BobaFettScumAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Illicit },
                    factionOverride: Faction.Scum
                );

                ModelInfo.SkinName = "Boba Fett";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class BobaFettScumAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Reroll,
                GetNumberOfEnemyShipsAtRange1
            );
        }

        private bool IsDiceModificationAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Attack) || (Combat.AttackStep == CombatStep.Defence))
            {
                // Make sure Boba Fett doesn't use his ability if he has already succeeded in blocking the attack.
                if (GetNumberOfEnemyShipsAtRange1() > 0 && (Combat.CurrentDiceRoll.Blanks + Combat.CurrentDiceRoll.Focuses) > 0) result = true;
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
