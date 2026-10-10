
using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Script
{
    // [Flags]
    public enum WeaponAttributes
    {
        None = 0,
        Silenced = 1, // 0001
        Scoped = 2, // 0010
        Laser = 3 // 0100
    }

    public class Encoder : MonoBehaviour
    {
        [SerializeField] private WeaponAttributes weapon;
        [SerializedDictionary] public SerializedDictionary<WeaponAttributes, string> weaponDescriptions;
    }
}

