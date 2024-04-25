
using Unity.VisualScripting;
using UnityEngine;



public class PointDrone3D : MonoBehaviour
{
    public Color colorDrone;
    public float altitudeDrone;
    public int battery;
    public int signal;
    public string name;
    
    private GameObject arrowUp;
    private GameObject arrowDown;
    private bool isDroneSelected = false;
    void Start()
    {
        arrowUp = Resources.Load<GameObject>("Prefabs/UI/Components/ArrowUp");
        arrowDown = Resources.Load<GameObject>("Prefabs/UI/Components/ArrowDown");
        setColorDrone();
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).transform.localPosition = new Vector3(0, altitudeDrone, 0);
        setArrowsUp();
        setArrowsDown();
    }
    
    void Update()
    {
        faceCamera();
    }
    
    public void ConvertToGlobalColor()
    {
        colorDrone = new Color(colorDrone.r, colorDrone.g, colorDrone.b, 1);
    }
    
    public void faceCamera()
    {
        Quaternion child1Rotation = transform.GetChild(1).transform.rotation;
        transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
        transform.GetChild(1).transform.rotation = child1Rotation;
        transform.GetChild(4).transform.rotation = child1Rotation;
    }

    public void setArrowsUp()
    {
        for (float i = altitudeDrone-2; i > 0; i -= 5)
        {
            GameObject ArrowUp = Instantiate(arrowUp, transform);
            // on set la couleur des flèches
            ArrowUp.GetComponent<MeshRenderer>().material.SetColor("_Color", colorDrone);
            ArrowUp.transform.localPosition = new Vector3(0, i, 0);
        }
    }
    
    public void setArrowsDown()
    {
        for (float i = altitudeDrone+2; i < 200; i += 5)
        {
            GameObject ArrowDown = Instantiate(arrowDown, transform);
            // on set la couleur des flèches
            ArrowDown.GetComponent<MeshRenderer>().material.SetColor("_Color", colorDrone);
            ArrowDown.transform.localPosition = new Vector3(0, i, 0);
        }
    }

    public void setColorDrone()
    {
        ConvertToGlobalColor();
        transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(colorDrone.r, colorDrone.g, colorDrone.b, 0.8f));
        transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Color", colorDrone);
        transform.GetChild(2).GetComponent<MeshRenderer>().material.SetColor("_Color", colorDrone);
        transform.GetChild(4).GetComponent<MeshRenderer>().material.SetColor("_Color", colorDrone);
    }
    
    public void setDroneSelected(bool value)
    {
        isDroneSelected = value;
    }
    
    public bool getDroneSelected()
    {
        return isDroneSelected;
    }
}
