using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class KrassisTrelix : FiresprayClassPatrolCraft
        {
            public KrassisTrelix() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Krassis Trelix",
                    3,
                    70,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KrassisTrelixAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 153
                );

                ModelInfo.SkinName = "Krassis Trelix";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KrassisTrelixAbility : GenericAbility
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
            HostShip.ArcsInfo.GetArc<Arcs.ArcRear>().ShotPermissions.CanShootTorpedoes = isActive;
            HostShip.ArcsInfo.GetArc<Arcs.ArcRear>().ShotPermissions.CanShootMissiles = isActive;
            HostShip.ArcsInfo.GetArc<Arcs.ArcRear>().ShotPermissions.CanShootCannon = isActive;
        }

        private bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack && Combat.ChosenWeapon != HostShip.PrimaryWeapons;
        }

        private int GetAiPriority()
        {
            return 90;
        }
    }
}