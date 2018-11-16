using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class BobaFettSV : FiresprayClassPatrolCraft
        {
            public BobaFettSV() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Boba Fett",
                    5,
                    80,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.BobaFettSVAbility)
                );

                ModelInfo.SkinName = "Boba Fett";
                ShipInfo.Faction = Faction.Scum;

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 149;
            }
        }
    }
}

namespace Abilities.FirstEdition
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
                GetNumberOfEnemyShipsAtRange1
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