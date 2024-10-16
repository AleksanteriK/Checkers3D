using UnityEngine;

public class Spectatorcamera : MonoBehaviour
{
    public Camerahandler cameraHandler;
    float sensitivity = 5;
    float normal_speed = 5;
    float sprint_speed = 7;
    float current_speed;

    void Update()
    {
        if (cameraHandler.GetActiveCamera() == Camerahandler.ActiveCamera.Spectator)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Movement();
            Rotation();
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void Rotation()
    {
        Vector3 MouseInput = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        transform.Rotate(MouseInput * sensitivity * Time.deltaTime * 50);
        Vector3 EulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(EulerRotation.x, EulerRotation.y, 0);
    }

    void Movement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift))
        {
            current_speed = sprint_speed;
        }

        else
        {
            current_speed = normal_speed;
        }

        transform.Translate(input * current_speed * Time.deltaTime);
    }
}