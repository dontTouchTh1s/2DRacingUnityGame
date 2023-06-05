using UnityEngine;

namespace Car
{
    public class CarInputHandler : MonoBehaviour
    {
        //Components
        private CarController _carController;

        //Awake is called when the script instance is being loaded.
        private void Awake()
        {
            _carController = GetComponent<CarController>();
        }

        private void Update()
        {
            var inputVector = Vector2.zero;

            //Get input from Unity's input system.
            inputVector.x = Input.GetAxis("Horizontal");
            inputVector.y = Input.GetAxis("Vertical");

            //Send the input to the car controller.
            _carController.SetInputVector(inputVector);
        }
    }
}