using BoardTools;
using Bombs;
using GameCommands;
using GameModes;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class FinchDallow : Mg100StarFortress
        {
            public FinchDallow() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Finch Dallow",
                    4,
                    66,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.FinchDallowAbility)
                );

                ModelInfo.SkinName = "Cobalt";

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/43d9a0555f719a4cbe1ffe905fd38c46.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FinchDallowAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnBombWillBeDropped += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnBombWillBeDropped -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnBombWillBeDropped, AskToUseFinchDallowAbility);
        }

        private void AskToUseFinchDallowAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                NeverUseByDefault,
                PlaceBombInstead,
                infoText: "Do you want to place bomb touching your ship instead?"
            );
        }

        private void PlaceBombInstead(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            BombsManager.IsOverriden = true;

            PlaceBombTokenSubphase subphase = Phases.StartTemporarySubPhaseNew<PlaceBombTokenSubphase>("Place the bomb", Triggers.FinishTrigger);
            subphase.AbilityName = HostShip.PilotInfo.PilotName;
            subphase.Description = "Place the bomb touching your ship";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }
    }
}

namespace SubPhases
{
    public class PlaceBombTokenSubphase : GenericSubPhase
    {
        private static GameObject BombGO;

        public string AbilityName;
        public string Description;
        public IImageHolder ImageSource;

        private static bool IsInReposition;

        public override List<GameCommandTypes> AllowedGameCommandTypes
        {
            get { return new List<GameCommandTypes>() { GameCommandTypes.BombPlacement };}
        }

        public override void Start()
        {
            IsTemporary = true;

            Prepare();
            Initialize();

            UpdateHelpInfo();

            base.Start();
        }

        public override void Prepare()
        {

        }

        public override void Initialize()
        {
            RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            ShowDescription();

            CreateBombObject(Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation());

            StartDrag();
        }

        private void CreateBombObject(Vector3 bombPosition, Quaternion bombRotation)
        {
            GameObject prefab = (GameObject)Resources.Load(BombsManager.CurrentBomb.bombPrefabPath, typeof(GameObject));
            BombGO = (MonoBehaviour.Instantiate(prefab, bombPosition, bombRotation, Board.GetBoard()));
        }

        public void ShowDescription()
        {
            ShowSubphaseDescription(AbilityName, Description, ImageSource);
        }

        public override void Next()
        {
            FinishSubPhase();
        }

        private void FinishSubPhase()
        {
            HideSubphaseDescription();

            Action callback = Phases.CurrentSubPhase.CallBack;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase.Resume();
            callback();
        }

        public override void Update()
        {
            if (IsInReposition)
            {
                CheckPerformRotation();
                if (CameraScript.InputMouseIsEnabled) PerformDrag();
                /*if (CameraScript.InputTouchIsEnabled) PerformTouchDragRotate();
                CheckLimits();*/
            }
        }

        private void PerformDrag()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 pointerPosition = new Vector3(hit.point.x, 0f, hit.point.z);
                Vector3 bombCenterOffset = BombGO.transform.position - BombGO.transform.Find("Model").position;

                BombGO.transform.position = pointerPosition + bombCenterOffset;
            }
        }

        public void StartDrag()
        {
            Roster.SetRaycastTargets(false);

            IsInReposition = true;
            IsReadyForCommands = true;
            Roster.GetPlayer(RequiredPlayer).SetupBomb();
        }

        public override void ProcessClick()
        {
            IsInReposition = false;

            GameManagerScript.Instance.StartCoroutine(CheckBombPlacement());
        }

        private IEnumerator CheckBombPlacement()
        {
            ObstaclesStayDetectorForced detector = BombGO.GetComponentInChildren<ObstaclesStayDetectorForced>();
            detector.ReCheckCollisionsStart();
            yield return WaitForFrames(2);
            detector.ReCheckCollisionsFinish();

            if (detector.OverlapsCurrentShipNow)
            {
                GameCommand command = GeneratePlaceBombCommand(BombGO.transform.position, BombGO.transform.eulerAngles);
                GameMode.CurrentGameMode.ExecuteCommand(command);
            }
            else
            {
                Messages.ShowError("The bomb must be touching ship's base");
                IsInReposition = true;
            }
        }

        public static IEnumerator WaitForFrames(int frameCount)
        {
            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
        }

        public static GameCommand GeneratePlaceBombCommand(Vector3 position, Vector3 angles)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("positionX", position.x.ToString()); parameters.AddField("positionY", position.y.ToString()); parameters.AddField("positionZ", position.z.ToString());
            parameters.AddField("rotationX", angles.x.ToString()); parameters.AddField("rotationY", angles.y.ToString()); parameters.AddField("rotationZ", angles.z.ToString());
            return GameController.GenerateGameCommand(
                GameCommandTypes.BombPlacement,
                typeof(PlaceBombTokenSubphase),
                parameters.ToString()
            );
        }

        public static void FinishBombPlacement(Vector3 position, Vector3 angles)
        {
            IsInReposition = false;
            Phases.CurrentSubPhase.IsReadyForCommands = false;

            BombGO.transform.position = position;
            BombGO.transform.eulerAngles = angles;

            Roster.SetRaycastTargets(true);
            BombsManager.CurrentBomb.ActivateBombs(new List<GameObject>() { BombGO }, Phases.CurrentSubPhase.Next);
        }

        private void CheckPerformRotation()
        {
            if (Console.IsActive) return;

            CheckResetRotation();
            if (Input.GetKey(KeyCode.LeftControl))
            {
                RotateBy1();
            }
            else
            {
                RotateBy22_5();
            }
        }

        private void CheckResetRotation()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                BombGO.transform.localEulerAngles = Vector3.zero;
            }
        }

        private void RotateBy22_5()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                BombGO.transform.localEulerAngles += new Vector3(0, -22.5f, 0);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                BombGO.transform.localEulerAngles += new Vector3(0, 22.5f, 0);
            }
        }

        private void RotateBy1()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                BombGO.transform.localEulerAngles += new Vector3(0, -1f, 0);
            }

            if (Input.GetKey(KeyCode.E))
            {
                BombGO.transform.localEulerAngles += new Vector3(0, 1f, 0);
            }
        }
    }
}