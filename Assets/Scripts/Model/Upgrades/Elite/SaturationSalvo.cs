﻿using Upgrade;
using Ship;
using System.Linq;

namespace UpgradesList
{
    public class SaturationSalvo : GenericUpgrade
    {
        public SaturationSalvo() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Saturation Salvo";
            Cost = 1;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Host.OnAttackMissedAsAttacker += RegisterSaturationSalvoAbility;
        }

        private void RegisterSaturationSalvoAbility()
        {
            GenericSecondaryWeapon weapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (weapon != null)
            {
                if (weapon.Type == UpgradeType.Torpedo || weapon.Type == UpgradeType.Missile)
                {
                    Triggers.RegisterTrigger(
                        new Trigger() {
                            Name = "Saturation Salvo",
                            TriggerType = TriggerTypes.OnAttackMissed,
                            TriggerOwner = Host.Owner.PlayerNo,
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

                Board.ShipDistanceInformation shotInfo = new Board.ShipDistanceInformation(Combat.Defender, ship);

                if (shotInfo.Range == 1 && ship.Agility < (Combat.ChosenWeapon as GenericUpgrade).Cost)
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
            diceType = DiceKind.Attack;
            diceCount = 1;

            finishAction = FinishAction;
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
