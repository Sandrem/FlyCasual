using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using SubPhases;
using SubPhases.SecondEdition;
using Bombs;

namespace UpgradesList.SecondEdition
{
    public class ClusterMines : GenericContactMineSE
    {
        public ClusterMines() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Cluster Mines",
                UpgradeType.Device,
                cost: 8,
                charges: 1,
                cannotBeRecharged: true,
                subType: UpgradeSubType.Mine
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/c27f0dcda78915239450bedf5b931d86.png";

            bombPrefabPath = "Prefabs/Bombs/ClusterMinesCentral";

            bombSidePrefabPath = "Prefabs/Bombs/ClusterMinesSide";
            bombSideDistanceX = 4.05f;
            bombSideDistanceZ = 0.1264f;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Selection.ActiveShip = ship;

            ClusterMinesCheckSubPhase subphase = Phases.StartTemporarySubPhaseNew<ClusterMinesCheckSubPhase>(
                "Damage from " + UpgradeInfo.Name,
                delegate {
                    Phases.FinishSubPhase(typeof(ClusterMinesCheckSubPhase));
                    callBack();
                }
            );
            subphase.HostUpgrade = this;
            subphase.Start();
        }

        public override void PlayDetonationAnimSound(GenericDeviceGameObject bombObject, Action callBack)
        {
            int random = UnityEngine.Random.Range(1, 8);
            Sounds.PlayBombSound(bombObject, "Explosion-" + random);
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1, delegate { callBack(); });
        }
    }
}

namespace SubPhases.SecondEdition
{
    public class ClusterMinesCheckSubPhase : DiceRollCheckSubPhase
    {
        public GenericUpgrade HostUpgrade;

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 2;

            AfterRoll = FinishAction;
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
            Messages.ShowInfo("Cluster Mines: " + Selection.ActiveShip.PilotInfo.PilotName + " suffers damage.");

            DamageSourceEventArgs clusterMinesDamage = new DamageSourceEventArgs()
            {
                Source = HostUpgrade,
                DamageType = DamageTypes.BombDetonation
            };

            Selection.ActiveShip.Damage.TryResolveDamage(CurrentDiceRoll.DiceList, clusterMinesDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfo("Cluster Mines: " + Selection.ActiveShip.PilotInfo.PilotName + " suffers no damage.");
            CallBack();
        }
    }
}