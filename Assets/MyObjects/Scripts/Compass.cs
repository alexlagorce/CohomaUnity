using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Compass : MonoBehaviour
{
    private  GameObject pointCompassPrefab;
    private RawImage compassImage;
    private Transform mainCamera;
    private float compassUnit;
    private List<PointMarker> pointMarkers = new List<PointMarker>();

    private void Start()
    {
        mainCamera = Camera.main.transform;
        InitializeCompassImage();
    }

    private void InitializeCompassImage()
    {
        pointCompassPrefab = Resources.Load<GameObject>("Prefabs/UI/Components/PointCompass");
        compassImage = transform.GetChild(0).GetComponent<RawImage>();
        pointCompassPrefab.GetComponent<RectTransform>().position = Vector3.zero;

        float offsetX = 0.25f;
        float offsetY = 0f;
        float width = 1f;
        float height = 1f;

        compassImage.uvRect = new Rect(offsetX, offsetY, width, height);
        compassUnit = compassImage.rectTransform.rect.width / 360f;
    }

    private void Update()
    {
        UpdateCompassImage();
        UpdatePointMarkers();
    }

    private void UpdateCompassImage()
    {
        Vector3 forward = mainCamera.forward;
        forward.y = 0;

        float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;
        headingAngle = 5 * Mathf.RoundToInt(headingAngle / 5.0f);

        int displayAngle = Mathf.RoundToInt(headingAngle);
        compassImage.uvRect = new Rect(displayAngle / 360f, 0f, 1f, 1f);
    }

    private void UpdatePointMarkers()
    {
        foreach (PointMarker marker in pointMarkers)
        {
            RectTransform rectTransform = marker.image.rectTransform;
            rectTransform.anchoredPosition = GetPosOnCompass(marker);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 5f);

            Text distanceText = marker.image.GetComponentInChildren<Text>();
            distanceText.text = GetDistance(marker).ToString("F1") + "m";
        }
    }

    public void AddPointMarker(PointMarker marker, Color color)
    {
        GameObject newMarker = Instantiate(pointCompassPrefab, transform);
        Image markerImage = newMarker.GetComponent<Image>();
        marker.image = markerImage;
        markerImage.sprite = markerImage.sprite;

        pointMarkers.Add(marker);
        marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker);
        marker.image.rectTransform.anchoredPosition = new Vector2(marker.image.rectTransform.anchoredPosition.x, 5f);
        marker.image.color = color;
        newMarker.GetComponentInChildren<Text>().color = color;
    }

    private Vector2 GetPosOnCompass(PointMarker marker)
    {
        Vector2 playerPos = new Vector2(mainCamera.position.x, mainCamera.position.z);
        Vector2 playerForward = new Vector2(mainCamera.forward.x, mainCamera.forward.z);
        float angle = Vector2.SignedAngle(marker.position - playerPos, playerForward);
        return new Vector2(angle * compassUnit, -5f);
    }

    public void RemovePointMarker(PointMarker marker)
    {
        pointMarkers.Remove(marker);
        Destroy(marker.image.gameObject);
    }

    private float GetDistance(PointMarker marker)
    {
        return Vector2.Distance(new Vector2(mainCamera.position.x, mainCamera.position.z), marker.position);
    }
}
