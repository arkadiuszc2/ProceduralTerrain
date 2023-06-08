using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    private Vector2 coords;
    private float[,] elevationMap;
    private bool[,] riversMap;
    private float[,] moistureMap;
    private float[,] temperatureMap;

    public Map(Vector2 coords,float[,] elevMap, bool[,] rivMap, float[,] moistMap, float[,] tempMap) {
        this.coords = coords;
        this.elevationMap = elevMap;
        this.riversMap = rivMap;
        this.moistureMap = moistMap;
        this.temperatureMap = tempMap;
    }

    public Vector2 Coords {
        get { return coords; }
    }

    public float[,] ElevationMap {
        get { return elevationMap; }
    }

    public bool[,] RiversMap {
        get { return riversMap; }
    }

    public float[,] MoistureMap {
        get { return moistureMap; }
    }

    public float[,] TemperatureMap {
        get { return temperatureMap; }
    }
}
