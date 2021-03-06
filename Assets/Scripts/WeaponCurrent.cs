﻿using UnityEngine;
using System.Collections;

public class WeaponCurrent : MonoBehaviour {
	[SerializeField] public int weaponCurrent = new int();
	[SerializeField] public int weaponIndex = new int();
	public static WeaponCurrent Instance;

	void Awake() {
		Instance = this;
		Instance.weaponCurrent = 0; // Current slot in the weapon inventory (7 slots)
		Instance.weaponIndex = 0; // Current index to the weapon look-up tables
	}
}