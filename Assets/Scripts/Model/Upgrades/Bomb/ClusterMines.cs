using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;

namespace UpgradesList
{

    public class ClusterMines : GenericContactMine
    {

        public ClusterMines() : base()
        {
            Types.Add(UpgradeType.Bomb);
            Name = "Cluster Mines";
            Cost = 4;

            bombPrefabPath = "Prefabs/Bombs/ClusterMinesCentral";

            bombSidePrefabPath = "Prefabs/Bombs/ClusterMinesSide";
            bombSideDistanceX = 2.362f;
            bombSideDistanceZ = 0.0764f;

            IsDiscardedAfterDropped = true;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Selection.ActiveShip = ship;
            Phases.StartTemporarySubPhaseOld(
                "Damage from " + Name,
                typeof(SubPhases.ClusterMinesCheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.ClusterMinesCheckSubPhase));
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

    public class ClusterMinesCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            diceType = DiceKind.Attack;
            diceCount = 2;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            CurrentDiceRoll.RemoveAllFailures();
            if (!CurrentDiceRoll.IsEmpty)
            {
                foreach (Die die in CurrentDiceRoll.DiceList)
                {
                    if (die.Side == DieSide.Crit) die.SetSide(DieSide.Success);
                }

                SufferDamage();
            }
            else
            {
                NoDamage();
            }

        }

        private void SufferDamage()
        {
            Messages.ShowError("Cluster Mines: ship suffered damage");

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
                        Source = "Cluster Mines",
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
