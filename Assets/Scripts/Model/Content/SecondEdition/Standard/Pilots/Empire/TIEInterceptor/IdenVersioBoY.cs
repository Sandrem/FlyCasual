using Abilities.SecondEdition;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class IdenVersioBoY : TIEInterceptor
        {
            public IdenVersioBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Iden Versio",
                    "Battle of Yavin",
                    Faction.Imperial,
                    4,
                    6,
                    0,
                    charges: 2,
                    regensCharges: 1,
                    isLimited: true,
                    abilityType: typeof(IdenVersioBoYAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    isStandardLayout: true
                );

                AutoThrustersAbility oldAbility = (AutoThrustersAbility) ShipAbilities.First(n => n.GetType() == typeof(AutoThrustersAbility));
                //oldAbility.DeactivateAbility();
                ShipAbilities.Remove(oldAbility);
                ShipAbilities.Add(new SensitiveControlsBoYRealAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Predator));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Fanatic));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/7/76/Idenversio-battleofyavin.png";

                PilotNameCanonical = "idenversio-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IdenVersioBoYAbility : GenericAbility
    {
        private GenericShip curToDamage;

        public override void ActivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal += CheckIdenVersioAbilitySE;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal -= CheckIdenVersioAbilitySE;
        }

        private void CheckIdenVersioAbilitySE(GenericShip toDamage, DamageSourceEventArgs e)
        {
            curToDamage = toDamage;

            // Is the defender on our team? If not return.
            if (curToDamage.Owner.PlayerNo != HostShip.Owner.PlayerNo)
                return;

            if (!(curToDamage is Ship.SecondEdition.TIELnFighter.TIELnFighter))
                return;

            // If the defender is at range one of us we register our trigger to prevent damage.
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(curToDamage, HostShip);
            if (distanceInfo.Range <= 1)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, UseIdenVersioAbilitySE);
            }
        }

        private void UseIdenVersioAbilitySE(object sender, System.EventArgs e)
        {
            // Are there any non-crit damage results in the damage queue?
            if (HostShip.State.Charges >= 2)
            {
                // If there are we prompt to see if they want to use the ability.
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    delegate { HostShip.SpendCharges(2); BlankDamage(); },
                    descriptionLong: "Do you want to spend 2 Charges to prevent damage?",
                    imageHolder: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void BlankDamage()
        {
            curToDamage.AssignedDamageDiceroll.RemoveAll();
            DecisionSubPhase.ConfirmDecision();
        }


    }
}