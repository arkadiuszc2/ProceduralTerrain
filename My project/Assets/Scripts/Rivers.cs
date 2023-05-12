using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rivers : MonoBehaviour {
    public static bool[,] GenerateRiversMap(float[,] heightMap, int numRivers, int riverLength) {
        bool[,] riverMap = new bool[heightMap.GetLength(0), heightMap.GetLength(1)];

        List<Point> riversStartPoints = new List<Point>(); 

        for (int i = 0; i < numRivers; i++) {


            // losowanie zrodla rzeki na mapie
            int startX = UnityEngine.Random.Range(1, heightMap.GetLength(0) - 1);
            int startY = UnityEngine.Random.Range(1, heightMap.GetLength(1) - 1);

            // losowanie zrodla rzeki az do momentu gdy wylosuje sie ono w gorach
            int counter = 0;
            while ((heightMap[startX, startY] < 0.52f && counter < 100) || !isFarEnoughFromOtherStartPoint(riversStartPoints, startX, startY)) { // bazowo <0.62

                startX = UnityEngine.Random.Range(1, heightMap.GetLength(0) - 1);
                startY = UnityEngine.Random.Range(1, heightMap.GetLength(1) - 1);
                counter++;
            }

            //zabezpieczenie w wypadku nie znalezienia punktu o wysokosci > 0.62f
            if (counter == 1000) {
                return riverMap;
            }

            riversStartPoints.Add(new Point(startX, startY));

            RiverPoint startPoint = new RiverPoint(startX, startY, heightMap[startX, startY], 0.0f, null);

            //losowanie konca rzeki
            //TO DO: dodac losowanie punktow w sensowny sposob np. lokalne minima/maksima

            int endX = UnityEngine.Random.Range(1, heightMap.GetLength(0) - 1);
            int endY = UnityEngine.Random.Range(1, heightMap.GetLength(1) - 1);

            counter = 0;
            while (heightMap[endX, endY] > 0.39f && counter < 100) {
                endX = UnityEngine.Random.Range(1, heightMap.GetLength(0) - 1);
                endY = UnityEngine.Random.Range(1, heightMap.GetLength(1) - 1);
                counter++;
            }

            if (counter == 100) {
                return riverMap;
            }

            RiverPoint endPoint = new RiverPoint(endX, endY, heightMap[endX, endY], 0.0f, null);

            List<RiverPoint> riverPoints = aStarPathFinder(startPoint, endPoint, heightMap, riverMap);
            
            foreach (RiverPoint point in riverPoints) {
                riverMap[point.x, point.y] = true;
            }
        }

        return riverMap;
    }
    //moja implementacja algorytmu Astar zmodyfikowanego aby znajdowal sciezke dla rzeki
    private static List<RiverPoint> aStarPathFinder(RiverPoint startPoint, RiverPoint endPoint, float[,] heightMap, bool[,] riverMap) {
        List<RiverPoint> openList = new List<RiverPoint>();
        HashSet<RiverPoint> closedSet = new HashSet<RiverPoint>();

        openList.Add(startPoint);

        // inicjalizacja rozpatrywanego aktualnie punktu
        RiverPoint currentNode = null;
        float currentMinRiverHeight = 0.0f;
        while (openList.Count > 0) {
            openList.Sort(new RiverPointsComparer());
            currentNode = openList[0];
            //set new max riverpoint height, used to validate downhill flow of the river
            

            if (isRiverInSea(currentNode) || isRiverInOtherRiver(currentNode, riverMap)) {
                break;
            }
            Debug.Log("CurrentNode" + "x: " + currentNode.x + " y: " + currentNode.y + " height: " + currentNode.height +" distEnd: " + currentNode.distToEnd);
            closedSet.Add(currentNode);
            openList.Remove(currentNode);

            if (currentNode.Equals(endPoint) == true) {
                break;
            }

            List<RiverPoint> neighboursList = createNeighboursList(currentNode, heightMap, endPoint);

            foreach (RiverPoint neighbor in neighboursList) {
                Debug.Log("Neighbor" + "x: " + neighbor.x + " y: " + neighbor.y + " height: " + neighbor.height + " distToEnd: " + neighbor.distToEnd);
                if (closedSet.Contains(neighbor)) {//isPointWalkable(neighbor,)==false
                    continue;
                }
                if (openList.Contains(neighbor) == true) {
                    RiverPoint neighborInOpenList = openList.Find(point => point.x == neighbor.x && point.y == neighbor.y);
                    if (neighbor.fCost <= neighborInOpenList.fCost) {
                        openList.Remove(neighborInOpenList);
                        openList.Add(neighbor);
                    }
                } else {
                    openList.Add(neighbor);
                }

            }

        }

        List<RiverPoint> riverPoints = new List<RiverPoint>();
        RiverPoint validatedPoint = currentNode;
        //riverPoints.Add(validatedPoint);
        while (validatedPoint.parent!=null ) {
            riverPoints.Add(validatedPoint);
            validatedPoint = validatedPoint.parent;
        }



        return riverPoints;


    }

    private static List<RiverPoint> createNeighboursList(RiverPoint currentPoint, float[,] heightMap, RiverPoint endpoint) {
        List<RiverPoint> neighborsList = new List<RiverPoint>();
        int x = currentPoint.x;
        int y = currentPoint.y;


        if (checkIfCoordsInRange(x, y + 1, heightMap.GetLength(0), heightMap.GetLength(1))) {
            neighborsList.Add(new RiverPoint(x, y + 1, heightMap[x, y + 1], calculateDistToEnd(x, y + 1, endpoint), currentPoint));
        }
        if (checkIfCoordsInRange(x + 1, y, heightMap.GetLength(0), heightMap.GetLength(1))) {
            neighborsList.Add(new RiverPoint(x + 1, y, heightMap[x + 1, y], calculateDistToEnd(x + 1, y, endpoint), currentPoint));
        }
        if (checkIfCoordsInRange(x, y - 1, heightMap.GetLength(0), heightMap.GetLength(1))) {
            neighborsList.Add(new RiverPoint(x, y - 1, heightMap[x, y - 1], calculateDistToEnd(x, y - 1, endpoint), currentPoint));
        }
        if (checkIfCoordsInRange(x - 1, y, heightMap.GetLength(0), heightMap.GetLength(1))) {
            neighborsList.Add(new RiverPoint(x - 1, y, heightMap[x - 1, y], calculateDistToEnd(x - 1, y, endpoint), currentPoint));
        }

        return neighborsList;
    }

    private static float calculateDistToEnd(int x, int y, RiverPoint endpoint) {
        int endX = endpoint.x;
        int endY = endpoint.y;
        float dist = 0;

        dist = Mathf.Sqrt(Mathf.Pow(endX - x, 2) + Mathf.Pow(endY - y,2));
        return dist;
    }

    private static bool checkIfCoordsInRange(int x, int y, int xSize, int ySize) {
        if (x >= 0 && x <= 254) {
            if (y >= 0 && y <= 254) {
                return true;
            }
        }
        return false;
    }

    private static bool isRiverInSea(RiverPoint currentNode) {
        return currentNode.height < 0.39f;
    }

    private static bool isRiverInOtherRiver(RiverPoint currentNode, bool[,] riverMap) {
        return riverMap[currentNode.x, currentNode.y]==true;
    }

    private static bool isFarEnoughFromOtherStartPoint(List<Point> startPoints, int startX, int startY) {
        foreach (Point point in startPoints) { 
            if(Mathf.Abs(point.x - startX) < 10 && Mathf.Abs(point.y - startY) < 10) {
                return false;
            }
        }
        return true;
    }

}
