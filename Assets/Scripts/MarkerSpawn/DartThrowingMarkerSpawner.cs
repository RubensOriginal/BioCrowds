using Biocrowds.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartThrowingMarkerSpawner : MarkerSpawner
{
    public override IEnumerator CreateMarkers(GameObject _auxingPrefab, Transform _auxinsContainer, List<Cell> cells, List<Auxin> auxins)
    {
        _cellSize = cells[0].transform.localScale.x;

        _maxMarkersPerCell = Mathf.RoundToInt(MarkerDensity / (MarkerRadius * MarkerRadius));

        // Generate a number of markers for each Cell
        for (int c = 0; c < cells.Count; c++)
        {
            StartCoroutine(PopulateCell(_auxingPrefab, _auxinsContainer, cells[c], auxins, c));
        }

        yield break;
    }

    private IEnumerator PopulateCell(GameObject _auxingPrefab, Transform _auxinsContainer, Cell cell, List<Auxin> auxins, int cellIndex)
    {
        float cellHalfSize = (_cellSize / 2.0f) * (1.0f - (MarkerRadius/2f));

        // Set this counter to break the loop if it is taking too long (maybe there is no more space)
        int _tries = 0;

        for (int i = 0; i < _maxMarkersPerCell; i++)
        {
            // If counter is above maxMarkers * 5, breaks the sequence for this Cell
            if (_tries > _maxMarkersPerCell * 5)
                break;

            // Candidate position for new Marker
            float x = Random.Range(-cellHalfSize, cellHalfSize);
            float z = Random.Range(-cellHalfSize, cellHalfSize);
            Vector3 targetPosition = new Vector3(x, 0f, z) + cell.transform.position;

            if (HasObstacleNearby(targetPosition) || HasMarkersNearby(targetPosition, cell.Auxins) 
                || !IsOnNavmesh(targetPosition))
            {
                _tries++;
                i--;
                continue;
            }
           
            // Creates new Marker and sets its data
            GameObject newMarker = Instantiate(_auxingPrefab, targetPosition, Quaternion.identity, _auxinsContainer);
            Auxin _marker = newMarker.GetComponent<Auxin>();
            newMarker.transform.localScale = Vector3.one * MarkerRadius;
            newMarker.name = "Marker [" + cellIndex + "][" + i + "]";
            _marker.Cell = cell;
            _marker.Position = targetPosition;
            _marker.ShowMesh(SceneController.ShowAuxins);

            auxins.Add(_marker);
            cell.Auxins.Add(_marker);

            // Reset the tries counter
            _tries = 0;
        }
        yield break;
    }

}
