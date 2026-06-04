using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GPSFind : MonoBehaviour
{
    [SerializeField] private RectTransform safeAreaContainer;
    
    [SerializeField] private float targetLatitude = 1.333191f;
    [SerializeField] private float targetLongitude = -74.845643f;
    
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private Image temperatureIndicator;
    [SerializeField] private RectTransform directionArrow;

    [SerializeField] private float foundRadius = 5f;
    [SerializeField] private float hotRadius = 20f;
    [SerializeField] private float warmRadius = 50f;
    [SerializeField] private float lukewarmRadius = 200f;
    [SerializeField] private float coldRadius = 500f;

    private bool found;
    private Rect lastSafeArea;

    private readonly Color colorCold = new Color(0f, 0f, 1f);
    private readonly Color colorLukewarm = new Color(0.5f, 1f, 0f);
    private readonly Color colorWarm = new Color(1f, 1f, 0f);
    private readonly Color colorHot = new Color(1f, 0f, 0f);

    private IEnumerator Start()
    {
        //if (!Input.location.isEnabledByUser) yield break;
        
        Input.location.Start(5f, 2f);

        int timeout = 20;

        Debug.Log(Input.location.status);
        
        while (Input.location.status == LocationServiceStatus.Stopped && timeout > 0)
        {
            yield return new WaitForSeconds(1f);
            timeout--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            yield break;
        }

        Debug.Log(Input.location.status);
        
        StartCoroutine(TrackLocation());
    }
    
    private IEnumerator TrackLocation()
    {
        while (!found)
        {
            var data = Input.location.lastData;
            float distance = DistanceMeters(data.latitude, data.longitude, targetLatitude, targetLongitude);
            
            UpdateDistanceDisplay(distance, data.horizontalAccuracy);
            UpdateColorIndicator(distance);
            UpdateDirectionArrow(data.latitude, data.longitude);
            
            if (distance <= foundRadius)
            {
                found = true;
                Input.location.Stop();
            }
        
            yield return new WaitForSeconds(1f);
            
        }
    }

    private void UpdateDistanceDisplay(float distance, float horizontalAccuracy)
    {
        distanceText.text = distance >= 1000f ? $"{distance / 1000:F1} km away" : $"{ distance:F0}";
        accuracyText.text = $"GPS accuracy: ±{horizontalAccuracy:F0} m";
    }

    private void UpdateColorIndicator(float distance)
    {
        Color target;
        if (distance > coldRadius) target = colorCold;
        else if (distance > lukewarmRadius) target = colorLukewarm;
        else if (distance > warmRadius) target = colorWarm;
        else target = colorHot;
        
        temperatureIndicator.color = Color.Lerp(temperatureIndicator.color, target, 0.1f);
    }

    private void UpdateDirectionArrow(float latitude, float longitude)
    {
        if (directionArrow == null) return;
        float bearing = Bearing(latitude, longitude, targetLatitude, targetLongitude);
        directionArrow.localEulerAngles = new Vector3(0, 0, -bearing);
    }

    private float DistanceMeters(float lat1, float lon1, float lat2, float lon2)
    {
        const float R = 6371000f;
        float dLat = (lat2 - lat1) * Mathf.Deg2Rad;
        float dLon = (lon2 - lon1) * Mathf.Deg2Rad;
        float a = Mathf.Sin(dLat / 2f) * Mathf.Sin(dLat / 2f) +
                  Mathf.Cos(lat1 * Mathf.Deg2Rad) * Mathf.Cos(lat2 * Mathf.Deg2Rad) *
                  Mathf.Sin(dLon / 2f) * Mathf.Sin(dLon / 2f);
        return R * 2f * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1f - a));
    }
    
    private float Bearing(float lat1, float lon1, float lat2, float lon2)
    {
        float dLon = (lon2 - lon1) * Mathf.Deg2Rad;
        float y = Mathf.Sin(dLon) * Mathf.Cos(lat2 * Mathf.Deg2Rad);
        float x = Mathf.Cos(lat1 * Mathf.Deg2Rad) * Mathf.Sin(lat2 * Mathf.Deg2Rad) -
                  Mathf.Sin(lat1 * Mathf.Deg2Rad) * Mathf.Cos(lat2 * Mathf.Deg2Rad) * Mathf.Cos(dLon);
        return ((Mathf.Atan2(y, x) * Mathf.Rad2Deg) + 360f) % 360f;
    }

    private void OnDestroy()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Input.location.Stop();
        }
    }
}
