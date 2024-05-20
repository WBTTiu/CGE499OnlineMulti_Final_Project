using System;
using UnityEngine;

    public class TreasureCollision : MonoBehaviour
    {
        [SerializeField] private GameObject _winUI;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _winUI.SetActive(true);
                FirstPersonController[] fpc = FindObjectsOfType<FirstPersonController>();
                foreach (var c in fpc)
                {
                    c.enabled = false;
                }
            }
        }
    }
