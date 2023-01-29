using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class FennRauRebel : FangFighter
        {
            public FennRauRebel() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Fenn Rau",
                    "Mandalorian Protector",
                    Faction.Rebel,
                    6,
                    6,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.FennRauRebelFangAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    },
                    skinName: "Fenn Rau"
                );

                PilotNameCanonical = "fennrau-rebelalliance";

                ImageUrl = "https://i.imgur.com/czHjZ4D.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FennRauRebelFangAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnCombatActivationGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnCombatActivationGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Tools.IsSameTeam(HostShip, ship)
                && BoardState.IsInRange(HostShip, ship, 1, 2)
                && ship.Tokens.GetNonLockRedTokens().Count >= 1
                && HasEnemyInFrontAtR1(ship))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, AskToRemoveRedNonLockToken);
            }
        }

        private bool HasEnemyInFrontAtR1(GenericShip ship)
        {
            foreach (GenericShip enemyShip in ship.Owner.EnemyShips.Values)
            {
                if (ship.SectorsInfo.RangeToShipBySector(enemyShip, Arcs.ArcType.Front) == 1) return true;
            }

            return false;
        }

        private void AskToRemoveRedNonLockToken(object sender, EventArgs e)
        {
            FennRauRebelRemoveRedTokenAbilityDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<FennRauRebelRemoveRedTokenAbilityDecisionSubPhase>(
                "Fenn Rau: You may remove 1 non-lock red token",
                Triggers.FinishTrigger
            );
            subphase.ImageSource = HostShip;
            subphase.AbilityHostShip = HostShip;
            subphase.RemoveOnlyNonLocks = true;
            subphase.Start();
        }

        private class FennRauRebelRemoveRedTokenAbilityDecisionSubPhase : RemoveRedTokenDecisionSubPhase
        {
            public GenericShip AbilityHostShip;

            public override void PrepareCustomDecisions()
            {
                DescriptionShort = AbilityHostShip.PilotInfo.PilotName;
                DescriptionLong = "You may remove 1 non-lock red token";

                DecisionOwner = Selection.ThisShip.Owner;
                DefaultDecisionName = decisions.First().Name;
            }
        }
    }
}