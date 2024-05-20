using System;
using UnityEngine;

namespace DoorPuzzle
{
    public class PressurePadBridge : MonoBehaviour
    {
        [SerializeField] private Bridges _bridges;
        [SerializeField] private GameObject _pressedPadPlayer;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && _pressedPadPlayer == null)
            {
                // move down
                transform.position =
                    new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
                // change color
                //GetComponentInChildren<Renderer>().material.color = Color.green;
                //send notification
                _bridges.IsMovingDown = false;
                _bridges.IsMovingUp = true;
                _pressedPadPlayer = other.gameObject;
                StopCoroutine(_bridges.MoveBridgesDown());
                StartCoroutine(_bridges.MoveBridgesUp());
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && _pressedPadPlayer == other.gameObject)
            {
                // move down
                transform.position =
                    new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
                // change color
                //GetComponentInChildren<Renderer>().material.color = Color.red;
                //send notification
                _bridges.IsMovingUp = false;
                _bridges.IsMovingDown = true;
                _pressedPadPlayer = null;
                StopCoroutine(_bridges.MoveBridgesUp());
                StartCoroutine(_bridges.MoveBridgesDown());

            }
        }
    }
}