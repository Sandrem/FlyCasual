using Abilities.SecondEdition;
using BoardTools;
using Ship;
using SubPhases;
using Tokens;
using Upgrade;
using Conditions;
using System.Linq;
using ActionsList;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class NabooHandmaiden : NabooRoyalN1Starfighter
        {
            public NabooHandmaiden() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Naboo Handmaiden",
                    "Regal Ruse",
                    Faction.Republic,
                    1,
                    4,
                    8,
                    limited: 2,
                    abilityText: "Setup: After placing forces, assign the Decoyed condition to 1 friendly ship other than Naboo Handmaiden.",
                    abilityType: typeof(NabooHandmaidenAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Modification
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/ad/b6/adb64448-5777-4fd3-8311-293207d7103b/swz40_naboo-handmaiden.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NabooHandmaidenAbility : GenericAbility
    {
        protected virtual string Prompt
        {
            get
            {
                return "Assign the Decoyed condition to 1 friendly ship other than Naboo Handmaiden.";
            }
        }
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterNabooHandmaidenAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterNabooHandmaidenAbility;
        }
        private void RegisterNabooHandmaidenAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = HostShip.ShipId + ": Assign \"Decoyed\" condition",
                TriggerType = TriggerTypes.OnSetupEnd,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectNabooHandmaidenTarget,
            });
        }

        private void SelectNabooHandmaidenTarget(object Sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                  AssignDecoyed,
                  CheckRequirements,
                  GetAiDecoyedPriority,
                  HostShip.Owner.PlayerNo,
                  "Decoyed",
                  Prompt,
                  HostUpgrade
            );
        }

        protected virtual void AssignDecoyed()
        {
            // Remove decoyed from all friendly ships
            foreach(var kvp in Roster.AllShips)
            {
                GenericShip ship = kvp.Value;
                ship.Tokens.RemoveCondition(typeof(Decoyed));
            }
            TargetShip.Tokens.AssignCondition(new Decoyed(TargetShip) { SourceUpgrade = HostUpgrade });
            SelectShipSubPhase.FinishSelection();
        }

        protected virtual bool CheckRequirements(GenericShip ship)
        {
            var match = ship.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && ship.PilotInfo.PilotName != "Naboo Handmaiden";
            return match;
        }

        private int GetAiDecoyedPriority(GenericShip ship)
        {
            int result = 0;
            int isN1 = 0;
            if (ship.ShipInfo.ShipName == "Naboo Royal N-1 Starfighter") isN1 = 1;

            result += (ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost))*(1+isN1);

            return result;
        }        
    }
}

namespace Conditions
{
    public class Decoyed : GenericToken
    {
        public GenericUpgrade SourceUpgrade;

        public Decoyed(GenericShip host) : base(host)
        {
            Name = ImageName = "Decoyed Condition";
            Temporary = false;
            Tooltip = "https://images-cdn.fantasyflightgames.com/filer_public/7e/38/7e38aca8-b0ea-4ddc-8ec4-64efca1544c8/swz40_decoyed.png";
        }

        public override void WhenAssigned()
        {
            Host.OnGenerateDiceModifications += AddDecoyedResultModification;
        }

        public override void WhenRemoved()
        {
            Host.OnGenerateDiceModifications -= AddDecoyedResultModification;
        }

        private void AddDecoyedResultModification(GenericShip ship)
        {
            foreach(var kvp in Roster.AllShips.Where(v => v.Value.PilotInfo.PilotName == "Naboo Handmaiden"))
            {
                RegisterDecoy(kvp.Value);
            }
        }

        private void RegisterDecoy(GenericShip hm)
        {
            var name = "Decoyed: HM " + hm.ShipId;
            DecoyedAction action = new DecoyedAction()
            {
                HostShip = Host,
                SourceShip = hm,
                DiceModificationName = name,
                Name = name
            };
            Host.AddAvailableDiceModificationOwn(action);
        }
    }
}

namespace ActionsList
{
    public class DecoyedAction : GenericAction
    {
        public GenericShip SourceShip;

        public DecoyedAction() {}

        public override bool IsDiceModificationAvailable()
        {
            bool result = true;
            // mod only works on defense
            if (HostShip != Combat.Defender || Combat.AttackStep != CombatStep.Defence) result = false;

            // SourceShip in attack arc with evades to spend
            if (!new ShotInfo(Combat.Attacker, SourceShip, Combat.Attacker.PrimaryWeapons).InArc
                || !SourceShip.Tokens.HasToken(typeof(EvadeToken))) result = false;

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            SourceShip.Tokens.SpendToken(typeof(EvadeToken), () => SpendOrAddEvade());
            callBack();
        }

        private void SpendOrAddEvade()
        {
            if (HostShip.ShipInfo.ShipName == "Naboo Royal N-1 Starfighter")
            {
                Messages.ShowInfo("Naboo Handmaiden: added evade.");
                Combat.CurrentDiceRoll.AddDiceAndShow(DieSide.Success);
            }
            else
            {
                Messages.ShowInfo("Naboo Handmaiden: spent evade.");
                Combat.CurrentDiceRoll.ApplyEvade();
            }
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;
            var def = Combat.CurrentDiceRoll;
            var atk = Combat.DiceRollAttack;
            // Different choices depending on host ship
            if (HostShip.ShipInfo.ShipName == "Naboo Royal N-1 Starfighter")
            {
                if (atk.Successes > def.Successes) result = 89;
            }
            else
            {
                if (atk.Successes > def.Successes)
                {
                    if (def.Blanks > 0)
                    {
                        result = 65;
                    }
                    else if (def.Focuses > 0
                        && HostShip.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                    {
                        result = 65;
                    }
                    else if (Combat.DiceRollAttack.Focuses > 0)
                    {
                        result = 15;
                    }
                }
            }
            return result;
        }
    }
}