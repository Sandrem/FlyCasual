using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    // TODO: Attack twice

    public class ClusterMissiles : GenericSecondaryWeapon
    {
        public ClusterMissiles() : base()
        {
            Type = UpgradeSlot.Missiles;

            Name = "Cluster Missiles";
            ShortName = "Cluster Missiles";
            ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/1/10/Cluster_Missiles.png";
            Cost = 4;

            MinRange = 1;
            MaxRange = 2;
            AttackValue = 3;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);
        }

        public override bool IsShotAvailable(Ship.GenericShip anotherShip)
        {
            bool result = true;

            if (isDiscarded) return false;

            int distance = Actions.GetFiringRange(Host, anotherShip);
            if (distance < MinRange) return false;
            if (distance > MaxRange) return false;

            if (!Actions.InArcCheck(Host, anotherShip)) return false;

            if (!Actions.HasTargetLockOn(Host, anotherShip)) return false;

            return result;
        }

        public override void PayAttackCost()
        {
            char letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
            Combat.Attacker.SpendToken(typeof(Tokens.BlueTargetLockToken), letter);
            Combat.Defender.RemoveToken(typeof(Tokens.RedTargetLockToken), letter);

            Discard();
        }

    }

}