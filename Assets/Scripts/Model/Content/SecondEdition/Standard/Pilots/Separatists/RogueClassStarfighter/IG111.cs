using Abilities.SecondEdition;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class IG111 : RogueClassStarfighter
        {
            public IG111() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "IG-111",
                    "One Eye",
                    Faction.Separatists,
                    1,
                    5,
                    21,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.IG111Ability),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Droid
                    }
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                DeadToRights oldAbility = (DeadToRights)ShipAbilities.First(n => n.GetType() == typeof(DeadToRights));
                oldAbility.DeactivateAbility();
                ShipAbilities.Remove(oldAbility);
                ShipAbilities.Add(new NetworkedCalculationsAbility());

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/ig111.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IG111Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            if (TargetsForAbilityExist(FilterTargets))
            {
                HostShip.OnAttackFinish += RegisterTrigger;
            }
        }

        private void RegisterTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, StartSelectShipSubphase);
            HostShip.OnAttackFinish -= RegisterTrigger;
        }

        private void StartSelectShipSubphase(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                SufferDamage,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "You may gain 1 deplete token to causee 1 enemy ship in your bullseye arc to suffer 1 damage",
                imageSource: HostShip
            );
        }

        private int GetAiPriority(GenericShip ship)
        {
            return (100) - (ship.State.ShieldsCurrent + ship.State.HullCurrent);
        }

        private void SufferDamage()
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " assigned a damage to " + Combat.Defender.PilotInfo.PilotName);
            SubPhases.SelectShipSubPhase.FinishSelectionNoCallback();
            DamageSourceEventArgs ig111damage = new DamageSourceEventArgs()
            {
                Source = HostShip,
                DamageType = DamageTypes.CardAbility
            };

            HostShip.Tokens.AssignToken(
                new Tokens.DepleteToken(HostShip),
                Triggers.FinishTrigger);

            TargetShip.Damage.TryResolveDamage(1, ig111damage, delegate { });

        }

        private bool FilterTargets(GenericShip ship)
        {
            return ship.Owner.PlayerNo != HostShip.Owner.PlayerNo
                && HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.Bullseye);
        }
    }
}