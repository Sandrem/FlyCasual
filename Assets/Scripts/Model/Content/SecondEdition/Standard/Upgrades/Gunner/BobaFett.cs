using Ship;
using Upgrade;
using System;
using Tokens;
using System.Collections.Generic;
using BoardTools;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class BobaFettGunner : GenericUpgrade
    {
        public BobaFettGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Boba Fett",
                UpgradeType.Gunner,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum, Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.BobaFettGunnerAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b6/39/b6395ed5-2a9c-46fd-9945-b906224aa05d/swz82_a1_upgrade_boba-fett.png";

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(233, 12)
            );

            NameCanonical = "bobafett-gunner";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class BobaFettGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Boba Fett",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Success
            );
        }

        private bool IsAvailable()
        {
            int shipsInAttackArc = 0;
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                ShotInfoArc shotInfoArc = new ShotInfoArc(HostShip, ship, Combat.ArcForShot);
                if (shotInfoArc.InArc) shipsInAttackArc++;
            }

            return Combat.AttackStep == CombatStep.Attack
                && shipsInAttackArc == 1
                && Combat.DiceRollAttack.HasResult(DieSide.Focus);
        }

        private int GetAiPriority()
        {
            return 55;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}