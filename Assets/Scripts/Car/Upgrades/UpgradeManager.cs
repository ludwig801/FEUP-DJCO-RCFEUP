using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Car.Upgrades {

	public class UpgradeManager : MonoBehaviour {

		public Car Car;
		private ICollection<Upgrade> _upgrades;

		public void Save() {
			_upgrades = new List<Upgrade> ();

			UpgradeWriter.Save(Car, _upgrades);
		}

	}

}

