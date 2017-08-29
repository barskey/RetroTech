using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	public List <GameObject> weapons = new List <GameObject> ();
	public GameObject armour;
	[HideInInspector]
	public GameObject equipped;
	[HideInInspector]
	public Weapon weapon;

	private GameObject attachPoint;
	private List <GameObject> equippedWeapons = new List <GameObject> ();

	void Awake ()
	{
		attachPoint = GameObject.Find ("weaponAttach");

		// Instantiate and disable all the weapons from the inventory
		for (int i = 0; i < weapons.Count; i++) {
			// if there is a weapon in that slot...
			if (weapons [i]) {
				equippedWeapons.Add (GameObject.Instantiate (weapons[i], attachPoint.transform));
				equippedWeapons[i].SetActive (false);
			}
		}

		EquipWeapon (0); // Equip the first weapon by default

	}

	public void EquipWeapon (int index)
	{
		if (equippedWeapons[index]) { // if there is a weapon to quip
			if (equipped)
				equipped.SetActive (false); // deactivate the currently active weapon

			equipped = equippedWeapons[index]; // store a reference to it
			equipped.SetActive (true); // enable it
			weapon = equipped.GetComponent <Weapon> (); // get the script component for this weapon
		}
	}
}
