using Ship;
using Upgrade;
using System.Collections.Generic;
using ActionsList;
using BoardTools;

namespace UpgradesList.FirstEdition
{
    public class ConcordDawnProtector : GenericUpgrade
    {
        public ConcordDawnProtector() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Concord Dawn Protector",
                UpgradeType.Title,
                cost: 1,          
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.ProtectorateStarfighter.ProtectorateStarfighter)),
                abilityType: typeof(Abilities.FirstEdition.ConcordDawnProtectorAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ConcordDawnProtectorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += TryAddConcordDawnProtectorDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= TryAddConcordDawnProtectorDiceModification;
        }

        private void TryAddConcordDawnProtectorDiceModification(GenericShip host)
        {
            GenericAction newAction = new ConcordDawnProtectorDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{
    public class ConcordDawnProtectorDiceModification : GenericAction
    {
        public ConcordDawnProtectorDiceModification()
        {
            Name = DiceModificationName = "Concord Dawn Protector";
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            result = 110;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence && Combat.ShotInfo.InArc)
            {
                DistanceInfo shipDistance = new DistanceInfo(Combat.Attacker, Combat.Defender);
                if (shipDistance.Range == 1)
                {
                    ShotInfo shotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
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
