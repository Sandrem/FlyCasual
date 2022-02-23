﻿using Arcs;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.VCX100LightFreighter
    {
        public class KananJarrus : VCX100LightFreighter
        {
            public KananJarrus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kanan Jarrus",
                    3,
                    73,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KananJarrusPilotAbility),
                    force: 2,
                    extraUpgradeIcon: UpgradeType.ForcePower,
                    seImageNumber: 74
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KananJarrusPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += CheckPilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= CheckPilotAbility;
        }

        private void CheckPilotAbility()
        {
            bool friendly = HostShip.Owner.PlayerNo == Combat.Defender.Owner.PlayerNo;
            bool hasForceTokens = HostShip.State.Force > 0;
            // according to the rules reference FAQ, a ship is never in its own arc
            bool inArc = Board.IsShipInArc(HostShip, Combat.Defender) && HostShip != Combat.Defender;

            if (friendly && hasForceTokens && inArc)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskDecreaseAttack);
            }
        }

        private void AskDecreaseAttack(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                DecreaseAttack,
                descriptionLong: "Do you want to spend a force token? (If you do, the attacker rolls 1 fewer attack die)",
                imageHolder: HostShip
            );
        }

        private void DecreaseAttack(object sender, EventArgs e)
        {
            RegisterDecreaseNumberOfAttackDice();
            HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
        }

        private void RegisterDecreaseNumberOfAttackDice()
        {
            Combat.Attacker.AfterGotNumberOfAttackDice += DecreaseNumberOfAttackDice;
        }

        private void DecreaseNumberOfAttackDice(ref int diceCount)
        {
            diceCount--;
            Combat.Attacker.AfterGotNumberOfAttackDice -= DecreaseNumberOfAttackDice;
        }

        private bool IsDefenderMe()
        {
            return Combat.Defender.ShipId == HostShip.ShipId;
        }

        private bool IsDefenderInMyMobileArc()
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, Combat.Defender, HostShip.PrimaryWeapons);
            return shotInfo.InArcByType(ArcType.SingleTurret);
        }
    }
}