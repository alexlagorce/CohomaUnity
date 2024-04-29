using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using MyObjects.Scripts;

public class PointMarkerGenerator : MonoBehaviour
{
    public GameObject rightController;
    private XRRayInteractor RightXRRayInteractor;
    private TextMeshPro descriptionRayText;
    private GameObject pointMarkerRed3D;
    private GameObject pointMarkerRed2D;
    Compass compass;
    private GameObject pointMarkerBlue3D;
    private GameObject pointMarkerBlue2D;

    private List<GameObject> listPointRed3D = new List<GameObject>();
    private List<GameObject> listPointRed2D = new List<GameObject>();
    private List<PointMarker> listMarkerRed = new List<PointMarker>();
    
    private List<GameObject> listPointBlue3D = new List<GameObject>();
    private List<GameObject> listPointBlue2D = new List<GameObject>();
    private List<PointMarker> listMarkerBlue = new List<PointMarker>();
    
    public InputActionReference Trigger_button;
    public InputActionReference Grip_button;
    public InputActionReference Y_button;
    
    private PointModeEnum[] pointModes = new PointModeEnum[] { PointModeEnum.UI, PointModeEnum.CIBLE, PointModeEnum.CARACTERISTIQUE };
    private int currentPointMode;
    private Gradient originalColorGradient;
    private Gradient colorGradientRed;
    private Gradient colorGradientBlue;
    private int incrementRed = 0;
    private int incrementBlue = 0;

    void Start()
    {
        InitializeObjects();   
    }

    void InitializeObjects()
    {
        
        pointMarkerRed3D = Resources.Load<GameObject>("Prefabs/UI/Components/PointMarkerRed3D");
        pointMarkerRed2D = Resources.Load<GameObject>("Prefabs/UI/Components/PointMarkerRed2D");
        pointMarkerBlue3D = Resources.Load<GameObject>("Prefabs/UI/Components/PointMarkerBlue3D");
        pointMarkerBlue2D = Resources.Load<GameObject>("Prefabs/UI/Components/PointMarkerBlue2D");
        
        compass = GameObject.Find("Compass").GetComponent<Compass>();
        RightXRRayInteractor = rightController.transform.GetChild(2).GetComponent<XRRayInteractor>();
        descriptionRayText = rightController.transform.GetChild(4).GetComponent<TextMeshPro>();
        originalColorGradient = RightXRRayInteractor.gameObject.GetComponent<XRInteractorLineVisual>().validColorGradient;
        colorGradientRed = new Gradient();
        colorGradientRed.SetKeys(
            new GradientColorKey[] { new GradientColorKey(UnityEngine.Color.red, 0.0f), new GradientColorKey(UnityEngine.Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        colorGradientBlue = new Gradient();
        colorGradientBlue.SetKeys(
            new GradientColorKey[] { new GradientColorKey(UnityEngine.Color.blue, 0.0f), new GradientColorKey(UnityEngine.Color.blue, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        currentPointMode = 0;
        descriptionRayText.text = "";
    }

    void Update()
    {
        HandlePointMode();

        if ((Input.GetKeyDown(KeyCode.Mouse0)||Trigger_button.action.triggered ) && (currentPointMode == 1 || currentPointMode == 2))
        {
            HandleLeftMouseClick();
        }

        if ((Input.GetKeyDown(KeyCode.Mouse1)||Grip_button.action.triggered ) && (currentPointMode == 1 || currentPointMode == 2))
        {
            HandleRightMouseClick();
        }
    }

    void HandlePointMode()
    {
        if (Input.GetKeyDown(KeyCode.L) || Y_button.action.triggered)
        {
            EditPointMode();
        }
    }

    void EditPointMode()
    {
        toggleNextPointMode();
        
        if (currentPointMode == 1)
        {
            SetPointModeCible();
        }

        if (currentPointMode == 2)
        {
            setPointModeCaracteristique();
        }
        
        if (currentPointMode == 0)
        {
            SetDefaultSettings();
        }
    }

    void SetPointModeCible()
    {
        RightXRRayInteractor.gameObject.GetComponent<XRInteractorLineVisual>().validColorGradient = colorGradientRed;
        RightXRRayInteractor.gameObject.GetComponent<XRRayInteractor>().raycastMask = LayerMask.GetMask("Map");

        ToggleUISwitch("Fullmap Toggle", false);
        ToggleUISwitch("ScrollDroneToggle", false);
        descriptionRayText.text = "Point Cible";
    }
    
    void setPointModeCaracteristique()
    {
        RightXRRayInteractor.gameObject.GetComponent<XRInteractorLineVisual>().validColorGradient = colorGradientBlue;
        RightXRRayInteractor.gameObject.GetComponent<XRRayInteractor>().raycastMask = LayerMask.GetMask("Map");
        
        ToggleUISwitch("Fullmap Toggle", false);
        ToggleUISwitch("ScrollDroneToggle", false);
        descriptionRayText.text = "Point Caractéristique";
    }

    void SetDefaultSettings()
    {
        RightXRRayInteractor.gameObject.GetComponent<XRInteractorLineVisual>().validColorGradient = originalColorGradient;
        RightXRRayInteractor.gameObject.GetComponent<XRRayInteractor>().raycastMask = LayerMask.GetMask("UI");
        descriptionRayText.text = "";
    }

    void ToggleUISwitch(string switchName, bool toggleValue)
    {
        Toggle toggle = GameObject.Find(switchName)?.GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.isOn = toggleValue;
        }
    }

    void HandleLeftMouseClick()
    {
        RaycastHit hit;
        bool isHitting = RightXRRayInteractor.TryGetCurrent3DRaycastHit(out hit);

        if (isHitting)
        {
            Create3DPointMarker(hit);
            Create2DPointMarker(hit);
        }
    }

    void Create3DPointMarker(RaycastHit hit)
    {
        if (currentPointMode == 1)
        {
            GameObject newObject = Instantiate(pointMarkerRed3D, hit.point, Quaternion.identity);
            Rigidbody rigidbody = newObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            newObject.GetComponentInChildren<TextMeshPro>().text = "Point Cible N°" + incrementRed;
            incrementRed++;
            listPointRed3D.Add(newObject);
            PointMarker marker = newObject.GetComponent<PointMarker>();
            marker.transform.position = hit.point;
            listMarkerRed.Add(marker);
            compass.AddPointMarker(marker, UnityEngine.Color.red);
        }
        
        if (currentPointMode == 2)
        {
            GameObject newObject = Instantiate(pointMarkerBlue3D, hit.point, Quaternion.identity);
            Rigidbody rigidbody = newObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            newObject.GetComponentInChildren<TextMeshPro>().text = "Point Caractéristique N°" + incrementBlue;
            incrementBlue++;
            listPointBlue3D.Add(newObject);
            PointMarker marker = newObject.GetComponent<PointMarker>();
            marker.transform.position = hit.point;
            listMarkerBlue.Add(marker);
            compass.AddPointMarker(marker, UnityEngine.Color.blue);
        }
    }

    void Create2DPointMarker(RaycastHit hit)
    {
        if (currentPointMode == 1)
        {
            GameObject newPoint2D = Instantiate(pointMarkerRed2D, hit.point, Quaternion.identity);
            newPoint2D.transform.position = new Vector3(hit.point.x, 29f, hit.point.z);
            listPointRed2D.Add(newPoint2D);
            StartCoroutine(StopAnimation(newPoint2D));
        }
        
        if (currentPointMode == 2)
        {
            GameObject newPoint2D = Instantiate(pointMarkerBlue2D, hit.point, Quaternion.identity);
            newPoint2D.transform.position = new Vector3(hit.point.x, 29f, hit.point.z);
            listPointBlue2D.Add(newPoint2D);
            StartCoroutine(StopAnimation(newPoint2D));
        }
    }

    IEnumerator StopAnimation(GameObject point2D)
    {
        yield return new WaitForSeconds(3);
        if (point2D != null)
        {
            point2D.GetComponentInChildren<Animator>().enabled = false;
        }
    }

    void HandleRightMouseClick()
    {
        RaycastHit hit;
        bool isHitting = RightXRRayInteractor.TryGetCurrent3DRaycastHit(out hit);
        if (isHitting)
        {
            RemoveExistingPoint(hit);
        }
    }

    /*
    void RemoveExistingPoint(RaycastHit hit)
    {
        if (listPoint3D.Count != 0 && listPoint2D.Count != 0 && listMarker.Count != 0)
        {
            foreach (var point in listPoint3D)
            {
                if (isNearExistingPoint(point, hit, 0.3f))
                {
                    int index = listPoint3D.IndexOf(point);
                    compass.removePointMarker(point.GetComponent<PointMarker>());
                    Destroy(listPoint2D[index]);
                    Destroy(point);
                    listPoint2D.Remove(listPoint2D[index]);
                    listMarker.Remove(listMarker[index]);
                    listPoint3D.Remove(point);
                }
            }
        }
    }
    */
    void RemoveExistingPoint(RaycastHit hit)
    {
        GameObject pointToRemove = null;

        if (currentPointMode == 1)
        {
            foreach (var point in listPointRed3D)
            {
                if (isNearExistingPoint(point, hit, 0.3f))
                {
                    pointToRemove = point;
                    break; // Sortir de la boucle car un seul point doit être retiré
                }
            }
            
            if (pointToRemove != null)
            {
                Destroy(pointToRemove);
                compass.RemovePointMarker(pointToRemove.GetComponent<PointMarker>());
                Destroy(listPointRed2D[listPointRed3D.IndexOf(pointToRemove)]);
                listPointRed2D.Remove(listPointRed2D[listPointRed3D.IndexOf(pointToRemove)]);
                listMarkerRed.Remove(listMarkerRed[listPointRed3D.IndexOf(pointToRemove)]);
                listPointRed3D.Remove(pointToRemove);
            }
        }
        
        if (currentPointMode == 2)
        {
            foreach (var point in listPointBlue3D)
            {
                if (isNearExistingPoint(point, hit, 0.3f))
                {
                    pointToRemove = point;
                    break; // Sortir de la boucle car un seul point doit être retiré
                }
            }
            if (pointToRemove != null)
            {
                Destroy(pointToRemove);
                compass.RemovePointMarker(pointToRemove.GetComponent<PointMarker>());
                Destroy(listPointBlue2D[listPointBlue3D.IndexOf(pointToRemove)]);
                listPointBlue2D.Remove(listPointBlue2D[listPointBlue3D.IndexOf(pointToRemove)]);
                listMarkerBlue.Remove(listMarkerBlue[listPointBlue3D.IndexOf(pointToRemove)]);
                listPointBlue3D.Remove(pointToRemove);
            }
        }
        
    }


    bool isNearExistingPoint(GameObject point, RaycastHit hit, float distance = 0.3f)
    {
        return Vector3.Distance(point.transform.position, hit.point) < distance;
    }
    
    public void toggleNextPointMode()
    {
        currentPointMode = (currentPointMode + 1) % pointModes.Length;
    }
}
