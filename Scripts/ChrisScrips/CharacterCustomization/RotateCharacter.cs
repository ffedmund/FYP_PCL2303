using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class RotateCharacter : MonoBehaviour
    {
        [SerializeField] private Transform _characterTransform;
        [SerializeField] private float _rotationSpeed;

        private void Awake()
        {
            _rotationSpeed = 100f;
        }
        
        private void Update()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            _characterTransform.Rotate(Vector3.up, horizontalInput * _rotationSpeed * Time.deltaTime);

            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X");
                _characterTransform.Rotate(Vector3.up, -mouseX * _rotationSpeed);
            }
        }


    }
}