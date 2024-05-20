using System;
using UnityEngine;

namespace DoorPuzzle
{
    public class PressurePadDoor : MonoBehaviour
    {
        [SerializeField] private Door _door;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // move down
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
                // change color
                //GetComponentInChildren<Renderer>().material.color = Color.green;
                //send notification
                if (!_door.IsPressurePad01Pressed && !_door.IsDoorMoved)
                {
                    _door.IsPressurePad01Pressed = true;
                }
                else if (!_door.IsPressurePad02Pressed && !_door.IsDoorMoved)
                {
                    _door.IsPressurePad02Pressed = true;
                    _door.IsDoorMoved = true;
                    StartCoroutine(_door.MoveDoor());
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // move up
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
                // change color
                //GetComponentInChildren<Renderer>().material.color = Color.red;
                //send notification
                if (_door.IsPressurePad01Pressed && !_door.IsDoorMoved)
                {
                    _door.IsPressurePad01Pressed = false;
                }
                else if (_door.IsPressurePad02Pressed && !_door.IsDoorMoved)
                {
                    _door.IsPressurePad02Pressed = false;
                }
            }

        }
    }
}
