using System.Collections.Generic;
using Ship;
using SubPhases;
using BoardTools;
using Content;
using Upgrade;
using Abilities.SecondEdition;
using System;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class BiggsDarklighterBoY : T65XWing
        {
            public BiggsDarklighterBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Biggs Darklighter",
                    "Battle of Yavin",
                    Faction.Rebel,
                    3,
                    5,
                    0,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BiggsDarklighterBoYAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech                        
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    },
                    skinName: "Biggs Darklighter",
                    isStandardLayout: true
                );

                ShipAbilities.Add(new HopeAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.AttackSpeed));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Selfless));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.R2F2BoY));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/8/8a/Biggsdarklighter-battleofyavin.png";

                PilotNameCanonical = "biggsdarklighter-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BiggsDarklighterBoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            flag = HasFriendlyShipsAtR1(ship);
        }

        private bool HasFriendlyShipsAtR1(GenericShip ship)
        {
            foreach (GenericShip anyShip in Roster.AllShips.Values)
            {
                if (IsFriendlyShipAtR1(anyShip)) return true;
            }

            return false;
        }

        private bool IsFriendlyShipAtR1(GenericShip anyShip)
        {
            if (!Tools.IsSameTeam(HostShip, anyShip)) return false;
            if (Tools.IsSameShip(HostShip, anyShip)) return false;

            DistanceInfo distInfo = new DistanceInfo(HostShip, anyShip);
            return distInfo.Range == 1;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, SelectTarget);
        }

        private void SelectTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility
            (
                CopyInitiative,
                IsFriendlyShipAtR1,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "You may choose 1 friendly ship at range 1. If you do, treat your initiative as equal to the chosen ship's initiative until the end of the Activation Phase."
            );
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.State.Initiative;
        }

        private void CopyInitiative()
        {
            new BiggsDarklighterBoYPilotSkillModifier(HostShip, TargetShip.State.Initiative);

            SelectShipSubPhase.FinishSelection();
        }

        private class BiggsDarklighterBoYPilotSkillModifier : IModifyPilotSkill
        {
            private GenericShip host;
            private int newPilotSkill;

            public BiggsDarklighterBoYPilotSkillModifier(GenericShip host, int newPilotSkill)
            {
                this.host = host;
                this.newPilotSkill = newPilotSkill;

                host.State.AddPilotSkillModifier(this);
                Phases.Events.OnActivationPhaseEnd_NoTriggers += RemoveBiggsDarklighterBoYPilotSkillModifier;
            }

            public void ModifyPilotSkill(ref int pilotSkill)
            {
                pilotSkill = newPilotSkill;
            }

            private void RemoveBiggsDarklighterBoYPilotSkillModifier()
            {
                host.State.RemovePilotSkillModifier(this);

                Phases.Events.OnActivationPhaseEnd_NoTriggers -= RemoveBiggsDarklighterBoYPilotSkillModifier;
            }
        }
    }
}
