using Ship;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class FlechetteTorpedoes : GenericSpecialWeapon
    {
        public FlechetteTorpedoes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Flechette Torpedoes",
                UpgradeType.Torpedo,
                cost: 2,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    spendsToken: typeof(BlueTargetLockToken),
                    discard: true
                ),
                abilityType: typeof(Abilities.FirstEdition.FlechetteTorpedoAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class FlechetteTorpedoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterFlechetteTorpedoes;

            //Ruling: "After you perform this attack, the defender receives 1 stress token if its hull value is "4" or lower."
            HostShip.OnAttackFinishAsAttacker += RegisterStress;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;

            HostShip.OnShotHitAsAttacker -= RegisterFlechetteTorpedoes;
            HostShip.OnAttackFinishAsAttacker -= RegisterStress;
        }

        private void RegisterFlechetteTorpedoes()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Flechette Torpedo Hit",
                    TriggerType = TriggerTypes.OnShotHit,
                    TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                    EventHandler = delegate { Triggers.FinishTrigger(); }
                });
            }
        }

        //When determing whether the defender receives a stress token from Flechette Torpedoes,
        //the defender's starting hull value is used (including any equipped Hull Upgrade cards),
        //not the defender's remaining hull points.
        private void RegisterStress(GenericShip hostShip)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Flechette Stress",
                TriggerType = TriggerTypes.OnAttackFinish,
                TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                EventHandler = delegate { AssignStressToDefender(); }
            });
        }

        private void AssignStressToDefender()
        {
            if (Combat.Defender.State.HullMax <= 4 && Combat.ChosenWeapon is UpgradesList.FirstEdition.FlechetteTorpedoes)
            {
                Messages.ShowInfoToHuman(string.Format("{0} received a Stress token from Flechette Torpedo", Combat.Defender.PilotInfo.PilotName));
                Combat.Defender.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}