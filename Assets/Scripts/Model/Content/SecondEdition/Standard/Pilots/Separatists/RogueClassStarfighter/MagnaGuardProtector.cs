using Content;
using System.Collections.Generic;
using System.Linq;
using Upgrade;
using Ship;
using Tokens;
using Abilities.SecondEdition;
using Conditions;
using SubPhases;
using BoardTools;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class MagnaGuardProtector : RogueClassStarfighter
        {
            public MagnaGuardProtector() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "MagnaGuard Protector",
                    "Implacable Escort",
                    Faction.Separatists,
                    4,
                    5,
                    18,
                    limited: 2,
                    abilityType: typeof(Abilities.SecondEdition.MagnaGuardProtectorAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
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

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/magnaguardprotector.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MagnaGuardProtectorAbility : GenericAbility
    {
        protected virtual string Prompt
        {
            get
            {
                return "Assign the Guarded condition to 1 friendly ship other than MagnaGuard Protector.";
            }
        }
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterMagnaGuardProtectorAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterMagnaGuardProtectorAbility;
        }

        private void RegisterMagnaGuardProtectorAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = HostShip.ShipId + ": Assign \"Guarded\" condition",
                TriggerType = TriggerTypes.OnSetupEnd,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectMagnaGuardProtectorTarget,
            });
        }

        private void SelectMagnaGuardProtectorTarget(object Sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                  AssignGuarded,
                  CheckRequirements,
                  GetAiGuardedPriority,
                  HostShip.Owner.PlayerNo,
                  "Guarded",
                  Prompt,
                  HostUpgrade
            );
        }

        protected virtual void AssignGuarded()
        {
            // Remove Guarded from all friendly ships
            foreach (var kvp in Roster.AllShips)
            {
                GenericShip ship = kvp.Value;
                ship.Tokens.RemoveCondition(typeof(Guarded));
            }
            TargetShip.Tokens.AssignCondition(new Guarded(TargetShip) { SourceUpgrade = HostUpgrade });
            SelectShipSubPhase.FinishSelection();
        }

        protected virtual bool CheckRequirements(GenericShip ship)
        {
            var match = Tools.IsSameTeam(ship, HostShip)
                && ship.PilotInfo.PilotName != "MagnaGuard Protector";
            return match;
        }

        private int GetAiGuardedPriority(GenericShip ship)
        {
            int result = 0;

            result += (ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost));

            return result;
        }
    }
}

namespace Conditions
{
    public class Guarded : GenericToken
    {
        public GenericUpgrade SourceUpgrade;
        public Guarded(GenericShip host) : base(host)
        {
            Name = ImageName = "Guarded Condition";
            Temporary = false;
            Tooltip = "https://infinitearenas.com/xw2/images/conditions/guarded.png";
        }

        public override void WhenAssigned()
        {
            Host.OnAttackStartAsDefender += CheckConditions;
        }

        public override void WhenRemoved()
        {
            Host.OnAttackStartAsDefender -= CheckConditions;
        }

        private void CheckConditions()
        {
            if (!Board.IsShipInArcByType(Combat.Attacker, Host, Arcs.ArcType.Bullseye))
            {
                Host.AfterGotNumberOfDefenceDice += RollExtraDice;
            }
        }

        private void RollExtraDice(ref int count)
        {
            int extraDice = Board.GetShipsInArcAtRange(Combat.Attacker, Combat.ArcForShot.ArcType, new UnityEngine.Vector2(Combat.ChosenWeapon.WeaponInfo.MinRange, Combat.ChosenWeapon.WeaponInfo.MaxRange), Team.Type.Enemy)
                .FindAll(s => s.PilotInfo.PilotName.Equals("MagnaGuard Protector") && (s.Tokens.HasToken<CalculateToken>() || s.Tokens.HasToken<EvadeToken>())).Count;
            if (extraDice > 0)
            {
                Messages.ShowInfo(Host.PilotInfo.PilotName + " is \"Guarded\" and gains +" + extraDice + " attack die");
                count += extraDice;
            }

            Host.AfterGotNumberOfAttackDice -= RollExtraDice;
        }
    }
}