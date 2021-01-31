using UnityEngine;

namespace SquadBuilderNS
{
    public class SquadBuilderSkinsView
    {
        public void ShowSkinsPanel()
        {
            string prefabPath = "Prefabs/SquadBuilder/SkinSelectionPanel";
            GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
            GameObject contentGO = GameObject.Find("UI/Panels/SkinsPanel/Content/Scroll View/Viewport/Content").gameObject;

            int shipsCount = Global.SquadBuilder.CurrentSquad.Ships.Count;
            contentGO.GetComponent<RectTransform>().sizeDelta = new Vector2(shipsCount * 600 + ((shipsCount + 1) * 20), 0);

            SquadBuilderView.DestroyChildren(contentGO.transform);
            SquadBuilderView.DestroyChildren(GameObject.Find("PreviewsHolder").transform);
            int i = 0;
            foreach (SquadListShip ship in Global.SquadBuilder.CurrentSquad.Ships)
            {
                GameObject newSkinPanel = GameObject.Instantiate(prefab, contentGO.transform);
                newSkinPanel.GetComponent<SkinSelectionPanelScript>().Initialize(ship.Instance, i++);
            }
        }

        private void SetSkinForShip(SquadListShip ship, string skinName)
        {
            ship.Instance.ModelInfo.SkinName = skinName;
        }
    }
}
