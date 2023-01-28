using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool isCamera2D;
    public bool movingTo2D = false;
    public bool movingTo3D = false;
    [SerializeField] private float cameraMovementSpeed = 10f;
    [SerializeField] private float cameraRotationSpeed = 200f;

    [SerializeField] private GameObject Ship3D;
    [SerializeField] private GameObject healthUI;
    [SerializeField] private AudioClip warpSFX;
    [SerializeField] private ParticleSystem warpVFX;

    private Vector3 position2D = new Vector3(0, 0, -10);
    private Vector3 rotation2D = new Vector3(0, 0, 0);

    private Vector3 position3D = new Vector3(0, 3, -5);
    private Vector3 rotation3D = new Vector3(15, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        transform.position = position2D;
        transform.eulerAngles = rotation2D;
        Camera.main.orthographic = true;
        isCamera2D = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingTo2D)
        {
            MoveTowardLocation(false);
        }
        else if (movingTo3D)
        {
            MoveTowardLocation(true);
        }

    }

    private void MoveTowardLocation(bool is2D)
    {
        if (is2D)
        {
            var targetPosition = position3D;
            var movementThisFrame = cameraMovementSpeed * Time.deltaTime;
            var rotationThisFrame = cameraRotationSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementThisFrame);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rotation3D), rotationThisFrame);
            if (transform.position == targetPosition && transform.eulerAngles == rotation3D)
            {
                transform.parent = Ship3D.transform;
                movingTo3D = false;
            }
        }
        else
        {
            var targetPosition = position2D;
            var movementThisFrame = cameraMovementSpeed * Time.deltaTime;
            var rotationThisFrame = cameraRotationSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementThisFrame);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rotation2D), rotationThisFrame);
            if (transform.position == targetPosition && transform.eulerAngles == rotation2D)
            {
                movingTo2D = false;
            }
        }
    }

    public void CameraInto2D()
    {
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("2D");
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("3D"));
        Camera.main.orthographic = true;
        transform.parent = null;
        healthUI.SetActive(false);
        StartCoroutine(DoVFX());
        Ship3D.transform.eulerAngles = new Vector3(0, 0, 0);
        movingTo2D = true;
        isCamera2D = true;
    }

    public void CameraInto3D()
    {
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("3D");
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("2D"));
        Camera.main.orthographic = false;
        healthUI.SetActive(true);
        StartCoroutine(DoVFX());
        movingTo3D = true;
        isCamera2D = false;
    }

    IEnumerator DoVFX()
    {
        AudioSource.PlayClipAtPoint(warpSFX, transform.position);
        warpVFX.Play();
        yield return new WaitForSeconds(0.5f);
        warpVFX.Stop();
    }
}
