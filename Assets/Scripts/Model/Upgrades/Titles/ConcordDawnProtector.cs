using Ship;
using Ship.ProtectorateStarfighter;
using UnityEngine;
using Upgrade;
using Abilities;
using ActionsList;

namespace UpgradesList
{
    public class ConcordDawnProtector : GenericUpgrade
    {
        public ConcordDawnProtector() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Concord Dawn Protector";
            Cost = 1;

            UpgradeAbilities.Add(new ConcordDawnProtectorAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is ProtectorateStarfighter;
        }
    }
}

namespace Abilities
{
    public class ConcordDawnProtectorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += TryAddConcordDawnProtectorDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= TryAddConcordDawnProtectorDiceModification;
        }

        private void TryAddConcordDawnProtectorDiceModification(GenericShip host)
        {
            GenericAction newAction = new ConcordDawnProtectorDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
            host.AddAvailableActionEffect(newAction);
        }
    }
}

namespace ActionsList
{
    public class ConcordDawnProtectorDiceModification : GenericAction
    {
        public ConcordDawnProtectorDiceModification()
        {
            Name = EffectName = "Concord Dawn Protector";
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            result = 110;

            return result;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence && Combat.ShotInfo.InArc)
            {
                Board.ShipDistanceInfo shipDistance = new Board.ShipDistanceInfo(Combat.Attacker, Combat.Defender);
                if (shipDistance.Range == 1)
                {
                    Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
                    if (shotInfo.InArc)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.AddDice(DieSide.Success).ShowWithoutRoll();
            Combat.CurrentDiceRoll.OrganizeDicePositions();

            callBack();
        }
    }

}
