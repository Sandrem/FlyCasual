using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;

namespace UpgradesList
{

    public class ProximityMines : GenericContactMine
    {

        public ProximityMines() : base()
        {
            Type = UpgradeType.Bomb;
            Name = "Proximity Mines";
            Cost = 3;

            bombPrefabPath = "Prefabs/Bombs/ProximityMine";

            IsDiscardedAfterDropped = true;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Selection.ActiveShip = ship;
            Phases.StartTemporarySubPhaseOld(
                "Damage from " + Name,
                typeof(SubPhases.ProximityMinesCheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.ProximityMinesCheckSubPhase));
                    callBack();
                });
        }

        public override void PlayDetonationAnimSound(GameObject bombObject, Action callBack)
        {
            int random = UnityEngine.Random.Range(1, 8);
            Sounds.PlayBombSound(bombObject, "Explosion-" + random);
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(1, delegate { callBack(); });
        }

    }

}

namespace SubPhases
{

    public class ProximityMinesCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            diceType = DiceKind.Attack;
            diceCount = 3;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            CurrentDiceRoll.RemoveAllFailures();
            if (!CurrentDiceRoll.IsEmpty)
            {
                SufferDamage();
            }
            else
            {
                NoDamage();
            }

        }

        private void SufferDamage()
        {
            Messages.ShowError("Proximity Mines: ship suffered damage");

            for (int i = 0; i < CurrentDiceRoll.DiceList.Count; i++)
            {
                Selection.ActiveShip.AssignedDamageDiceroll.AddDice(CurrentDiceRoll.DiceList[i].Side);

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Suffer damage",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                    EventHandler = Selection.ActiveShip.SufferDamage,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = "Proximity Mines",
                        DamageType = DamageTypes.BombDetonation
                    },
                    Skippable = true
                });
            }

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("No damage");
            CallBack();
        }
    }

}
