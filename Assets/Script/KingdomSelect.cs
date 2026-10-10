using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Script
{
    public class KingdomSelect : MonoBehaviour
    {
        [SerializeField]
        private GameObject kingdomPrefab, kingdomButtonPrefab;

        [SerializeField]
        private Transform kingdomParent, uiKingdomContainer;

        [SerializeField]
        private List<Kingdom> kingdoms = new List<Kingdom>();

        private Vector3 _cameraParentAngle = Vector3.zero;
        private int _currentIndex;

        private void Start()
        {
            foreach (var kingdom in kingdoms)
                SpawnKingdomPoint(kingdom);
        }

        private void SpawnKingdomPoint(Kingdom kingdom)
        {
            var kingdomGameObject = Instantiate(kingdomPrefab, kingdomParent);
            kingdomGameObject.transform.localEulerAngles = new Vector3(kingdom.x, kingdom.y, 0);

            SpawnKingdomButton(kingdom);
        }

        private void SpawnKingdomButton(Kingdom kingdom)
        {
            var kingdomButton = Instantiate(kingdomButtonPrefab, uiKingdomContainer).GetComponent<Button>();
            kingdomButton.onClick.AddListener((() => LookAtKingdom(kingdom)));
        }

        private void LookAtKingdom(Kingdom kingdom)
        {
            var cam = Camera.main;
            if (!cam) return;
            var cameraParent = cam.transform.parent;

            _cameraParentAngle.x = kingdom.x;
            _cameraParentAngle.y = kingdom.y;

            cameraParent.DOLocalRotate(_cameraParentAngle, 1);
        }

        private void FixedUpdate()
        {
            if (!Keyboard.current.anyKey.wasPressedThisFrame) return;

            if (Keyboard.current.aKey.wasPressedThisFrame)
                CycleValue(-1, 0, kingdoms.Count - 1);
            if (Keyboard.current.dKey.wasPressedThisFrame)
                CycleValue(1, 0, kingdoms.Count - 1);

            Debug.Log("Looking at kingdom " + _currentIndex);

            LookAtKingdom(kingdoms[_currentIndex]);
        }

        private void CycleValue(int delta, int min, int max)
        {
            _currentIndex += delta;
            if(_currentIndex < min)
                _currentIndex = max;
            else if(_currentIndex > max)
                    _currentIndex = min;
        }
    }
}

[Serializable]
public class Kingdom
{
    public string name;
    public float x, y;
}
