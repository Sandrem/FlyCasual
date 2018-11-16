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
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.KrassisTrelixAbility)
                );

                ModelInfo.SkinName = "Krassis Trelix";
                ShipInfo.Faction = Faction.Scum;

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 153;
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
            return Combat.AttackStep == CombatStep.Attack && Combat.ChosenWeapon != HostShip.PrimaryWeapon;
        }

        private int GetAiPriority()
        {
            return 90;
        }
    }
}