using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	public List <GameObject> weapons = new List <GameObject> ();
	public GameObject armor;
	[HideInInspector]
	public GameObject equipped;
	[HideInInspector]
	public Weapon weapon;
	[HideInInspector]
	public WeaponBar weaponBar;

	private GameObject attachPoint;
	private List <GameObject> equippedWeapons = new List <GameObject> ();

	void Awake ()
	{
		attachPoint = GameObject.Find ("weaponAttach"); // attachment point for instantiating weapons
		weaponBar = GameObject.Find ("WeaponBar").GetComponent <WeaponBar> (); // get the weapon bar script so we can communicate with it

		// for each weapon available...
		for (int i = 0; i < weapons.Count; i++) {
			// if a weapon is added...
			if (weapons [i]) {
				equippedWeapons.Add (GameObject.Instantiate (weapons[i], attachPoint.transform)); // instantiate
				equippedWeapons[i].SetActive (false); // and disbale initially

				weaponBar.ActivateSlot (i, weapons [i]); // set up the image on the weapon bar slot
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

			weaponBar.ActivateSlot (index, equipped);
			weapon = equipped.GetComponent <Weapon> (); // get the script component for this weapon
		}
	}
}
