using RuleSets;
using Ship;

namespace Ship
{
    namespace Kihraxz
    {
        public class CaptainJostero : Kihraxz, ISecondEditionPilot
        {
            public CaptainJostero()
            {
                PilotName = "Captain Jostero";
                PilotSkill = 3;
                Cost = 43;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new Abilities.SecondEdition.CaptainJosteroAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // nope
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainJosteroAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnDamageInstanceResolvedGlobal += CheckJosteroAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDamageInstanceResolvedGlobal += CheckJosteroAbility;
        }

        private void CheckJosteroAbility(GenericShip damaged, DamageSourceEventArgs damage)
        {
            // Make sure the opposing ship is an enemy.
            if (damaged.Owner == HostShip.Owner)
                return;

            // If the ship is defending we're not interested.
            if (Combat.Defender == damaged || damage.DamageType == DamageTypes.ShipAttack)
                return;

            
        }

        private void RegisterCombat(object sender, System.EventArgs e)
        {
                Combat.StartAdditionalAttack(
                    HostShip,
                    AfterExtraAttackSubPhase,
                    null,
                    HostShip.PilotName,
                    "You may perform a primary weapon attack.",
                    HostShip.ImageUrl
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack one more time", HostShip.PilotName));
                Triggers.FinishTrigger();
            }
        }
    }
}
