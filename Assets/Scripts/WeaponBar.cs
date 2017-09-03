using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WeaponBar : MonoBehaviour {

	public const int weaponbarSize = 3;
	public Sprite inactiveBackground;
	public Sprite activeBackground;

	private int activeSlot = 0;
	private GameObject[] weaponSlot = new GameObject[weaponbarSize];

	private Color enabledText = new Color (0.67f, 0.2f, 0.2f);
	private Color disabledText = Color.gray;

	// Use this for initialization
	void Awake ()
	{
		// Get references to weapon slots in weapon bar
		weaponSlot [0] = GameObject.Find ("Slot1");
		weaponSlot [1] = GameObject.Find ("Slot2");
		weaponSlot [2] = GameObject.Find ("Slot3");
	}

	public void ActivateSlot (int index, GameObject weapon)
	{
		// deactivate the currently active slot
		DeactivateSlot (activeSlot);

		// set internal index to the new slot
		activeSlot = index;

		// set the background image of the current active slot
		weaponSlot [activeSlot].GetComponent <Image> ().sprite = activeBackground;

		// set the image of the current active slot
		Image newImage = weaponSlot [activeSlot].transform.Find ("Image").GetComponent <Image> ();
		newImage.sprite = weapon.GetComponent <SpriteRenderer> ().sprite;
		newImage.enabled = true;

		// set the rounds remaining color
		weaponSlot [activeSlot].transform.Find ("AmmoBackground").GetComponent <Image> ().color = enabledText;
	}

	private void DeactivateSlot (int slotNum)
	{
		// disbale the background
		weaponSlot [activeSlot].GetComponent <Image> ().sprite = inactiveBackground;

		// diable the rounds remaining color
		weaponSlot [activeSlot].transform.Find ("AmmoBackground").GetComponent <Image> ().color = disabledText;
	}

	public void SetAmmoLeft (int rounds)
	{
		weaponSlot [activeSlot].transform.Find ("Ammo").GetComponent <Text> ().text = rounds.ToString ();
	}

}
