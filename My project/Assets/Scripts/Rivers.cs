using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rivers : MonoBehaviour {
    public static bool[,] GenerateRiversMap(float[,] heightMap, int numRivers, int riverLength) {
        bool[,] riverMap = new bool[heightMap.GetLength(0), heightMap.GetLength(1)];

    
        for (int i = 0; i < numRivers; i++) {
             

             // losowanie zrodla rzeki na mapie
             int x = UnityEngine.Random.Range(1, heightMap.GetLength(0) - 1);
             int y = UnityEngine.Random.Range(1, heightMap.GetLength(1) - 1);

            // losowanie zrodla rzeki az do momentu gdy wylosuje sie ono w gorach
            int counter = 0;
            while (heightMap[x, y] < 0.62f && counter<100) {    
                x = UnityEngine.Random.Range(1, heightMap.GetLength(0) - 1);
                y = UnityEngine.Random.Range(1, heightMap.GetLength(1) - 1);
                counter++;
              }

            if(counter == 100) {
                return riverMap;
            }



            for (int j = 0; j < riverLength; j++) {
                //nie mozna wyjsc poza mape
                if (x == 0 || x == 254 || y == 0 || y == 254) {
                    break;
                }
                RiverPoint lowestNeigh = calcLowestNeigh(x, y, heightMap, riverMap);
                if(lowestNeigh == null) {
                    break;
                }
                x = lowestNeigh.x;
                y = lowestNeigh.y;

                // gdy rzeka wpadnie do zbiornika wodnego to konczymy
                if (heightMap[x, y]<0.39f) { 
                    break;
                }


                riverMap[x, y] = true;

           

            }
        }

        return riverMap;
    }

    private static RiverPoint calcLowestNeigh(int x, int y, float[,] heightMap, bool[,] riverMap) {
        // cases
        // 1) when its no point lowe then actual to go i choose the loweest and lower its high to slightly below actual point

        PriorityQueue pq = new PriorityQueue();

        RiverPoint[] newPoints = {
        new RiverPoint(x, y + 1, heightMap[x,y+1]),
        new RiverPoint(x + 1, y, heightMap[x + 1, y]),
        new RiverPoint(x, y - 1, heightMap[x,y-1]),
        new RiverPoint(x-1, y, heightMap[x-1,y])
        };

        for (int i = 0; i < 4; i++) {
            pq.Enqueue(newPoints[i]);
        }

        RiverPoint lowestNeigh = pq.Dequeue();

        while (riverMap[lowestNeigh.x, lowestNeigh.y] == true && pq.Count > 0) {
            lowestNeigh = pq.Dequeue();
        }

        // jesli nawet ostatni wylosowany punkt byl juz punktem rzeki to konczymy
        if(riverMap[lowestNeigh.x, lowestNeigh.y] == true){
            return null;
        }

        if (lowestNeigh.height > heightMap[x,y]) {
              heightMap[lowestNeigh.x, lowestNeigh.y] = heightMap[x, y] - 0.001f;  // stala wartosc o jaka jest zmniejszana wysokosc najnizszego punktu
            // lowestNeigh = BFS(lowestNeigh, riverMap); 
           // return null;
        }        

        return lowestNeigh;


    }

}
