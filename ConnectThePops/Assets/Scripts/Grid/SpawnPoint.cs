using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool isActive = true;
    public bool IsActive { get => isActive; set => isActive = value; }
    
    public Vector3 ground;
    public Vector3 Ground { get => ground; set => ground = value; }
    
    private List<SpawnPoint> neighbors = new List<SpawnPoint>();

    private List<SpawnPoint> closerNeighbors = new List<SpawnPoint>();
    public List<SpawnPoint> Neighbors { get => neighbors; set => neighbors = value; }
    public List<SpawnPoint> CloserNeighbors { get => closerNeighbors; set => closerNeighbors = value; }

    private SpawnPoint neighbor_UP;
    private SpawnPoint neighbor_RIGHT;
    private SpawnPoint neighbor_LEFT;
    private SpawnPoint neighbor_DOWN;
    private SpawnPoint neighbor_UP_LEFT;
    private SpawnPoint neighbor_UP_RIGHT;
    private SpawnPoint neighbor_DOWN_LEFT;
    private SpawnPoint neighbor_DOWN_RIGHT;

    public SpawnPoint Neighbor_UP { get => neighbor_UP; set => neighbor_UP = value; }
    public SpawnPoint Neighbor_RIGHT { get => neighbor_RIGHT; set => neighbor_RIGHT = value; }
    public SpawnPoint Neighbor_LEFT { get => neighbor_LEFT; set => neighbor_LEFT = value; }
    public SpawnPoint Neighbor_DOWN { get => neighbor_DOWN; set => neighbor_DOWN = value; }
    public SpawnPoint Neighbor_UP_LEFT { get => neighbor_UP_LEFT; set => neighbor_UP_LEFT = value; }
    public SpawnPoint Neighbor_UP_RIGHT { get => neighbor_UP_RIGHT; set => neighbor_UP_RIGHT = value; }
    public SpawnPoint Neighbor_DOWN_LEFT { get => neighbor_DOWN_LEFT; set => neighbor_DOWN_LEFT = value; }
    public SpawnPoint Neighbor_DOWN_RIGHT { get => neighbor_DOWN_RIGHT; set => neighbor_DOWN_RIGHT = value; }

    private void Start()
    {
        isActive = true;
        RefreshNeighbors();
    }

    public void RefreshNeighbors()
    {
        Neighbors.Clear();
        CloserNeighbors.Clear();
        Neighbors.Add(Neighbor_UP);
        Neighbors.Add(Neighbor_RIGHT);
        Neighbors.Add(Neighbor_LEFT);
        Neighbors.Add(Neighbor_DOWN);
        Neighbors.Add(Neighbor_UP_LEFT);
        Neighbors.Add(Neighbor_UP_RIGHT);
        Neighbors.Add(Neighbor_DOWN_LEFT);
        Neighbors.Add(Neighbor_DOWN_RIGHT);
        CloserNeighbors.Add(Neighbor_UP);
        CloserNeighbors.Add(Neighbor_RIGHT);
        CloserNeighbors.Add(Neighbor_LEFT);
        CloserNeighbors.Add(Neighbor_DOWN);
    }
}
