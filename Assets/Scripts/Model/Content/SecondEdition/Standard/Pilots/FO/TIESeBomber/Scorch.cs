using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESeBomber
    {
        public class Scorch : TIESeBomber
        {
            public Scorch() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Scorch\"",
                    "Jad Bean",
                    Faction.FirstOrder,
                    4,
                    4,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ScorchBomberPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                PilotNameCanonical = "scorch-tiesebomber";

                ImageUrl = "https://i.imgur.com/lK59dsa.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ScorchBomberPilotAbility : GenericAbility
    {
        GenericShip SufferedShip;

        public override void ActivateAbility()
        {
            AddDiceModification
            (
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Cancel,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Success },
                payAbilityCost: PlanToAssignStrainToDefender,
                isGlobal: true
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private void PlanToAssignStrainToDefender(Action<bool> callback)
        {
            SufferedShip = Combat.Defender;
            SufferedShip.OnAttackFinishAsDefender += RegisterGetStrainToken;
            callback(true);
        }

        private void RegisterGetStrainToken(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, GetStrainToken);
        }

        private void GetStrainToken(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: {SufferedShip.PilotInfo.PilotName} gains Strain token");

            SufferedShip.OnAttackFinishAsDefender -= RegisterGetStrainToken;

            SufferedShip.Tokens.AssignToken(typeof(Tokens.StrainToken), FinishAblity);
        }

        private void FinishAblity()
        {
            SufferedShip = null;
            Triggers.FinishTrigger();
        }

        private bool IsAvailable()
        {
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) return false;

            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (Combat.DiceRollAttack.RegularSuccesses == 0) return false;

            if (Combat.Attacker.Owner != HostShip.Owner) return false;

            DistanceInfo positionInfo = new DistanceInfo(HostShip, Combat.Attacker);
            if (positionInfo.Range > 1) return false;

            return true;
        }

        private int GetAiPriority()
        {
            throw new NotImplementedException();
        }
    }
}
