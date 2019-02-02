﻿using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class SaturationSalvo : GenericUpgrade
    {
        public SaturationSalvo() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Saturation Salvo",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.SaturationSalvoAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class SaturationSalvoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += RegisterSaturationSalvoAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= RegisterSaturationSalvoAbility;
        }

        private void RegisterSaturationSalvoAbility()
        {
            GenericSpecialWeapon weapon = Combat.ChosenWeapon as GenericSpecialWeapon;
            if (weapon != null)
            {
                if (weapon.HasType(UpgradeType.Torpedo) || weapon.HasType(UpgradeType.Missile))
                {
                    Triggers.RegisterTrigger(
                        new Trigger()
                        {
                            Name = "Saturation Salvo",
                            TriggerType = TriggerTypes.OnAttackMissed,
                            TriggerOwner = HostShip.Owner.PlayerNo,
                            EventHandler = SaturationSalvoDamage
                        });
                }
            }
        }

        private void SaturationSalvoDamage(object sender, System.EventArgs e)
        {
            var allShips = Roster.AllShips.Select(x => x.Value).ToList();

            foreach (GenericShip ship in allShips)
            {

                if (ship.ShipId == Combat.Defender.ShipId) continue;

                BoardTools.DistanceInfo shotInfo = new BoardTools.DistanceInfo(Combat.Defender, ship);

                if (shotInfo.Range == 1 && ship.State.Agility < (Combat.ChosenWeapon as GenericUpgrade).UpgradeInfo.Cost)
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = ship.ShipId + ": Saturation Salvo Damage Check",
                        TriggerType = TriggerTypes.OnAbilityDirect,
                        TriggerOwner = ship.Owner.PlayerNo,
                        EventHandler = StartSaturationSalvoCheckSubPhase,
                        Sender = ship
                    });
                }
            }

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        private void StartSaturationSalvoCheckSubPhase(object sender, System.EventArgs e)
        {
            Selection.ActiveShip = sender as GenericShip;
            Phases.StartTemporarySubPhaseOld(
                Selection.ActiveShip.ShipId + ": Saturation Salvo Check",
                typeof(SubPhases.SaturationSalvoCheckSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(SubPhases.SaturationSalvoCheckSubPhase));
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}

namespace SubPhases
{

    public class SaturationSalvoCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            switch (CurrentDiceRoll.DiceList[0].Side)
            {
                case DieSide.Blank:
                    NoDamage();
                    break;
                case DieSide.Focus:
                    NoDamage();
                    break;
                case DieSide.Success:
                    Messages.ShowErrorToHuman("Damage is dealt!");
                    SufferDamage();
                    break;
                case DieSide.Crit:
                    Messages.ShowErrorToHuman("Critical damage is dealt!");
                    SufferDamage();
                    break;
                default:
                    break;
            }
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("No damage");
            CallBack();
        }

        private void SufferDamage()
        {
            foreach (var dice in CurrentDiceRoll.DiceList)
            {
                Selection.ActiveShip.AssignedDamageDiceroll.DiceList.Add(dice);

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Saturation Salvo Damage",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                    EventHandler = Selection.ActiveShip.SufferDamage,
                    Skippable = true,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = "Saturation Salvo",
                        DamageType = DamageTypes.CardAbility
                    }
                });
            }

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, CallBack);
        }

    }
}
