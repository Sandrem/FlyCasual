using ActionsList;
using Arcs;
using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using UnityEngine;
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
                cost: 12,
                isLimited: true,
                addForce: 1,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.ReyGunnerAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Resistance,
                new Vector2(339, 1)
            );

            NameCanonical = "rey-gunner";
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
                HostShip.State.SpendForce(1, delegate { callback(true); });
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