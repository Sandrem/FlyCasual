using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class IonTorpedoes : GenericSpecialWeapon
    {
        public IonTorpedoes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Torpedoes",
                UpgradeType.Torpedo,
                cost: 5,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    spendsToken: typeof(BlueTargetLockToken),
                    discard: true
                ),
                abilityType: typeof(Abilities.FirstEdition.IonTorpedoesAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class IonTorpedoesAbility : GenericAbility
    {
        private List<GenericShip> shipsToAssignIon;

        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterIonTorpedoesHit;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnShotHitAsAttacker -= RegisterIonTorpedoesHit;
        }

        private void RegisterIonTorpedoesHit()
        {
            if (Combat.ChosenWeapon == this.HostUpgrade)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Ion Torpedo Hit",
                        TriggerType = TriggerTypes.OnShotHit,
                        TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                        EventHandler = delegate {
                            AssignIonTokens();
                        }
                    }
                );
            }
        }

        private void AssignIonTokens()
        {
            shipsToAssignIon = new List<GenericShip>();

            var ships = Roster.AllShips.Select(x => x.Value).ToList();

            foreach (GenericShip ship in ships)
            {
                BoardTools.DistanceInfo shotInfo = new BoardTools.DistanceInfo(Combat.Defender, ship);
                if (shotInfo.Range == 1)
                {
                    shipsToAssignIon.Add(ship);
                }
            }

            AssignIonTokensRecursive();
        }

        private void AssignIonTokensRecursive()
        {
            if (shipsToAssignIon.Count > 0)
            {
                GenericShip shipToAssignIon = shipsToAssignIon[0];
                shipsToAssignIon.Remove(shipToAssignIon);
                Messages.ShowErrorToHuman(shipToAssignIon.PilotInfo.PilotName + " gains an Ion Token");
                shipToAssignIon.Tokens.AssignToken(typeof(IonToken), AssignIonTokensRecursive);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}