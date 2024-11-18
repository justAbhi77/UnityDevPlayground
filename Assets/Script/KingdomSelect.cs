using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class KingdomSelect : MonoBehaviour
    {
        [SerializeField]
        private GameObject kingdomPrefab, kingdomButtonPrefab;
        
        [SerializeField]
        Transform kingdomParent, uiKingdomContainer;
        
        [SerializeField]
        List<Kingdom> kingdoms = new List<Kingdom>();

        private Vector3 _cameraParentAngle = Vector3.zero;
        private int _currentIndex = 0;
        
        void Start()
        {
            foreach (Kingdom kingdom in kingdoms)
                SpawnKingdomPoint(kingdom);
        }

        void SpawnKingdomPoint(Kingdom kingdom)
        {
            GameObject kingdomGameObject = Instantiate(kingdomPrefab, kingdomParent);
            kingdomGameObject.transform.localEulerAngles = new Vector3(kingdom.x, kingdom.y, 0);
            
            SpawnKingdomButton(kingdom);
        }

        void SpawnKingdomButton(Kingdom kingdom)
        {
            Button KingdomButton = Instantiate(kingdomButtonPrefab, uiKingdomContainer).GetComponent<Button>();
            KingdomButton.onClick.AddListener((() => LookAtKingdom(kingdom)));
        }
        public void LookAtKingdom(Kingdom kingdom)
        {
            if (Camera.main != null)
            {
                Transform cameraParent = Camera.main.transform.parent;

                _cameraParentAngle.x = kingdom.x;
                _cameraParentAngle.y = kingdom.y;

                cameraParent.DOLocalRotate(_cameraParentAngle, 1, RotateMode.Fast);
            }
        }

        void FixedUpdate()
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.A))
                    CycleValue(-1, 0, kingdoms.Count - 1);
                if(Input.GetKeyDown(KeyCode.D))
                    CycleValue(1, 0, kingdoms.Count - 1);
                
                Debug.Log("Looking at kingdom " + _currentIndex);
                
                LookAtKingdom(kingdoms[_currentIndex]);
            }
        }

        void CycleValue(int delta, int min, int max)
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
