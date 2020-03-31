using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleShelves : MonoBehaviour
{
    public GameObject Room;

    private bool ShelvesAreLoaded;
    private GameObject Shelves;

    void OnMouseDown()
    {
        if (!ShelvesAreLoaded)
		{
			GameObject prefab = Resources.Load<GameObject>("Prefabs/Scenes/Shelves");
			Shelves = Instantiate(prefab, Room.transform);
			ShelvesAreLoaded = true;
		}
		else
		{	
			GameObject.Destroy(Shelves);
			ShelvesAreLoaded = false;
		}
    }
}
