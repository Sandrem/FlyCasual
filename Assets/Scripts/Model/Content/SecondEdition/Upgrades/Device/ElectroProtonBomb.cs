using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using Bombs;
using SubPhases.SecondEdition;
using Tokens;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class ElectroProtonBomb : GenericTimedBombSE
    {
        public ElectroProtonBomb() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Electro-Proton Bomb",
                types: new List<UpgradeType> { UpgradeType.Device, UpgradeType.Modification },
                cost: 12,
                charges: 1,
                cannotBeRecharged: true,
                subType: UpgradeSubType.Bomb,
                seImageNumber: 65,
                limited: 1,
                restriction: new ActionBarRestriction(typeof(ReloadAction))
            );
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/27/b2/27b2f5f3-8f2c-4480-831d-d593cd9aa567/swz41_electro-proton_bomb.png";
            detonationRange = 2;
            bombPrefabPath = "Prefabs/Bombs/ElectroProtonBomb";
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Selection.ActiveShip = ship;
            var sufferEPBEffectSubphase = Phases.StartTemporarySubPhaseNew<ElectroProtonBombCheckSubPhase>(
                "Effects from " + UpgradeInfo.Name,
                () => {
                    Phases.FinishSubPhase(typeof(ElectroProtonBombCheckSubPhase));
                    callBack();
                }
            );
            sufferEPBEffectSubphase.HostUpgrade = this;            
            sufferEPBEffectSubphase.Start();
        }

        public override void PlayDetonationAnimSound(GenericDeviceGameObject bombObject, Action callBack)
        {
            BombsManager.CurrentDevice = this;

            Sounds.PlayBombSound(bombObject, "Explosion-7");
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Sparks").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1.4f, delegate { callBack(); });
        }

        public override void ActivateBombs(List<GenericDeviceGameObject> bombObjects, Action callBack)
        {
            base.ActivateBombs(bombObjects, callBack);
            foreach (var bombObject in bombObjects)
            {
                bombObject.Fuses++;
            }
        }

    }
}

namespace SubPhases.SecondEdition
{
    public class ElectroProtonBombCheckSubPhase : DiceRollCheckSubPhase
    {
        public GenericUpgrade HostUpgrade;

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 4;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            SufferEffects();
        }

        private void SufferEffects()
        {
            // When this device detonates, each ship and remote at range 0-2 rolls 4 attack dice. 
            // Each ship: 
            //  Blank: lose 1 shield
            //  Eye/Hit: gain 1 ion token
            //  Crit: gain 1 disarm token
            // Each remote:
            //  Blank: lose 1 shield.
            //  Eye/Hit: suffer 1 damage            

            Dictionary<string, int> effects = new Dictionary<string, int>
            {
                { "shield", 0 },
                { "ion", 0 },
                { "disarm", 0 },
                { "damage", 0 }
            };
            foreach(var die in CurrentDiceRoll.DiceList)
            {
                var ship = Selection.ActiveShip;
                string triggerName;
                EventHandler action;
                switch(die.Side)
                {
                    case DieSide.Blank:
                        triggerName = $"{ship.PilotInfo.PilotName}: Lose shield";
                        action = (s,e)=> LoseShield(ship);
                        effects["shield"]++;
                        break;
                    case DieSide.Focus:
                    case DieSide.Success:
                        triggerName = $"{ship.PilotInfo.PilotName}: Gain ion token";
                        action = (s, e) => GainToken<IonToken>(ship);
                        effects["ion"]++;
                        break;
                    case DieSide.Crit:
                        triggerName = $"{ship.PilotInfo.PilotName}: Gain disarm token";
                        action = (s, e) => GainToken<WeaponsDisabledToken>(ship);
                        effects["disarm"]++;
                        break;
                    default:
                        continue;
                }                
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = triggerName,
                    TriggerType = TriggerTypes.OnBombIsDetonated,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = action,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = HostUpgrade,
                        DamageType = DamageTypes.BombDetonation
                    }
                });
            }
            var effectMessages = effects
                .Where(kvp => kvp.Value > 0)
                .Select(kvp => $"{kvp.Value} {kvp.Key}")
                .ToArray();
            var effectsMessage = string.Join(", ", effectMessages);

            Messages.ShowInfo($"{Selection.ActiveShip.PilotInfo.PilotName}: {effectsMessage}");
            Triggers.ResolveTriggers(TriggerTypes.OnBombIsDetonated, CallBack);
        }

        protected void LoseShield(GenericShip target)
        {
            if (target.State.ShieldsCurrent > 0)
            {
                target.Damage.SufferRegularDamage(
                    new DamageSourceEventArgs()
                    {
                        Source = this,
                        DamageType = DamageTypes.BombDetonation
                    },
                    Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        protected void GainToken<T>(GenericShip target) 
            where T : GenericToken
        {
            target.Tokens.AssignToken(typeof(T), Triggers.FinishTrigger);
        }

    }

}