using ActionsList;
using Arcs;
using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ReyGunner : GenericUpgrade
    {
        public ReyGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Rey",
                UpgradeType.Gunner,
                cost: 14,
                isLimited: true,
                addForce: 1,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.ReyGunnerAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/e11aec8ae6ec855694947bc2f9d1917e.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class ReyGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Rey's Ability",
                IsDiceModificationAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide>() { DieSide.Blank },
                DieSide.Success,
                payAbilityCost: PayForce
            );
        }

        private bool IsDiceModificationAvailable()
        {
            GenericShip enemyShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Defender : Combat.Attacker;
            ShotInfo shotInfo = new ShotInfo(HostShip, enemyShip, HostShip.PrimaryWeapons);

            return HostShip.State.Force > 0
                && Combat.CurrentDiceRoll.Blanks > 0
                && shotInfo.InArcByType(ArcType.SingleTurret);
        }

        private int GetAiPriority()
        {
            return 45;
        }

        private void PayForce(Action<bool> callback)
        {
            if (HostShip.State.Force > 0)
            {
                HostShip.State.Force--;
                callback(true);
            }
            else
            {
                Messages.ShowError("ERROR: This ship has no Force to spend");
                callback(false);
            }
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
};