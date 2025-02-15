using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TerrainSpawner : MonoBehaviour
{
    public GameObject terrainPrefab;
    public ARRaycastManager raycastManager;

    public Vector3 positionOffset = new Vector3(0f, 0f, 0f);

    private GameObject spawnedTerrain;
    private Vector3 fixedSpawnPosition;
    private Quaternion fixedSpawnRotation;
    private bool isTerrainPlaced = false;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Update()
    {
        if (isTerrainPlaced)
            return;

        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            Vector3 adjustedPosition = hitPose.position + positionOffset;

            if (spawnedTerrain == null)
            {
                spawnedTerrain = Instantiate(terrainPrefab, adjustedPosition, hitPose.rotation);
            }
            else
            {
                // Move terrain to match plane (with offset)
                spawnedTerrain.transform.position = adjustedPosition;
                spawnedTerrain.transform.rotation = hitPose.rotation;
            }

            // Lock terrain on tap
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                fixedSpawnPosition = adjustedPosition; // Store the position with offset
                fixedSpawnRotation = hitPose.rotation;
                isTerrainPlaced = true;

                // Optional: If you need to fix position after locking
                spawnedTerrain.transform.position = fixedSpawnPosition;
                spawnedTerrain.transform.rotation = fixedSpawnRotation;

                Debug.Log($"Terrain locked at position: {fixedSpawnPosition}");
            }
        }
    }
}
