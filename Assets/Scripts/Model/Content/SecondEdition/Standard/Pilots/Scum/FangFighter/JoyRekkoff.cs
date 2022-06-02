using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class JoyRekkoff : FangFighter
        {
            public JoyRekkoff() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Joy Rekkoff",
                    "Skull Squadron Ace",
                    Faction.Scum,
                    4,
                    5,
                    11,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JoyRekkoffAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Torpedo
                    },
                    tags: new List<Tags>
                    {
                        Tags.Mandalorian
                    },
                    seImageNumber: 157
                );
            }
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class JoyRekkoffAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                HostShip.OnGenerateDiceModifications += AddAbilityActivation;
            }

            public override void DeactivateAbility()
            {
                HostShip.OnGenerateDiceModifications -= AddAbilityActivation;
            }

            private void AddAbilityActivation(GenericShip ship)
            {
                ship.AddAvailableDiceModificationOwn(new JoyRekkoffAbilityActivation());
            }

            private class JoyRekkoffAbilityActivation : ActionsList.GenericAction
            {
                public JoyRekkoffAbilityActivation()
                {
                    Name = DiceModificationName = "Joy Rekkoff";

                    IsNotRealDiceModification = true;
                }

                public override void ActionEffect(System.Action callBack)
                {
                    GenericSpecialWeapon torpedo = (GenericSpecialWeapon)HostShip.UpgradeBar.GetUpgradesOnlyFaceup().FirstOrDefault(n => n.UpgradeInfo.HasType(UpgradeType.Torpedo) && n.State.Charges > 0);
                    torpedo.State.SpendCharge();
                    Combat.Defender.Tokens.AssignCondition(typeof(Conditions.JoyRekkoffCondition));

                    callBack();
                }

                public override bool IsDiceModificationAvailable()
                {
                    return Combat.AttackStep == CombatStep.Attack
                        && HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n.UpgradeInfo.HasType(UpgradeType.Torpedo)
                        && n.State.Charges > 0);
                }

                public override int GetDiceModificationPriority()
                {
                    int defenderRollsDiceCount = Combat.Defender.State.Agility;
                    if (Combat.ShotInfo.IsObstructedByObstacle) defenderRollsDiceCount++;
                    if (Combat.ShotInfo.Range == 3) defenderRollsDiceCount++;

                    // don't use if there are no green dice
                    if (defenderRollsDiceCount == 0) return 0;

                    //don't use if overkill
                    if ((Combat.Defender.State.HullCurrent + Combat.Defender.State.ShieldsCurrent) + defenderRollsDiceCount - Combat.DiceRollAttack.Successes < 0)
                    {
                        return 0;
                    }

                    return 1;
                }
            }
        }
    }
}

namespace Conditions
{
    public class JoyRekkoffCondition : GenericToken
    {
        bool AgilityWasDecreased = false;

        public JoyRekkoffCondition(GenericShip host) : base(host)
        {
            Name = ImageName = "Debuff Token";
            TooltipType = typeof(Ship.SecondEdition.FangFighter.JoyRekkoff);

            Temporary = false;
        }

        public override void WhenAssigned()
        {
            if (Host.State.Agility != 0)
            {
                AgilityWasDecreased = true;

                Messages.ShowInfo("Joy Rekkoff causes " + Host.PilotInfo.PilotName + "'s Agility to be decreased by 1");
                Host.ChangeAgilityBy(-1);
            }

            Host.OnAttackFinishAsDefender += RemoveJoyRekkoffAbility;
        }

        private void RemoveJoyRekkoffAbility(GenericShip ship)
        {
            Host.Tokens.RemoveCondition(this);
        }

        public override void WhenRemoved()
        {
            if (AgilityWasDecreased)
            {
                Messages.ShowInfo("Joy Rekkoff: " + Host.PilotInfo.PilotName + "'s Agility has been restored");
                Host.ChangeAgilityBy(+1);
            }

            Host.OnAttackFinishAsDefender -= RemoveJoyRekkoffAbility;
        }
    }
}