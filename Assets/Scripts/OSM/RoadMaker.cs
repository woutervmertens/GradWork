﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GSD;
using GSD.Roads;

/*
    Copyright (c) 2017 Sloan Kelly

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

/// <summary>
/// Road infrastructure maker.
/// </summary>
class RoadMaker : InfrastructureBehaviour
{
    public Material RoadMaterial;

    public GSDRoadSystem RoadSystem; 

    /// <summary>
    /// Create the roads.
    /// </summary>
    /// <returns></returns>
    IEnumerator Start()
    {
        // Wait for the map to become ready
        while (!map.IsReady)
        {
            yield return null;
        }

        // Iterate through the roads and build each one
        foreach (var way in map.ways.FindAll((w) => { return w.IsRoad; }))
        {
            CreateObject(way, RoadMaterial, way.Name);
            yield return null;
        }

        
    }

    void Create()
    {
        // Iterate through the roads and build each one
        foreach (var way in map.ways.FindAll((w) => { return w.IsRoad; }))
        {
            CreateObject(way, RoadMaterial, way.Name);
        }
    }

    void Update()
    {
        if (ready != map.IsReady)
        {
            ready = map.IsReady;
            if(ready) Create();
        }
    }

    protected override void OnObjectCreated(OsmWay way, Vector3 origin, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs, List<int> indices)
    {
        //GameObject rdObject = RoadSystem.AddRoad();
        //GSDRoad road = rdObject.GetComponent<GSDRoad>();

        for (int i = 1; i < way.NodeIDs.Count; i++)
        {
            OsmNode p1 = map.nodes[way.NodeIDs[i - 1]];
            OsmNode p2 = map.nodes[way.NodeIDs[i]];

            Vector3 s1 = p1 - origin;
            Vector3 s2 = p2 - origin;

            //GSDConstruction.CreateNode(road, false, default(Vector3), false, true, s1 + way.LocalPos);
            //GSDConstruction.CreateNode(road, false, default(Vector3), false, true, s2 + way.LocalPos);

            Vector3 diff = (s2 - s1).normalized;

            // https://en.wikipedia.org/wiki/Lane
            // According to the article, it's 3.7m in Canada
            // Europe: 2.5 - 3.25m, highways 3.5 - 4m
            var cross = Vector3.Cross(diff, Vector3.up) * 3.5f * way.Lanes;

            // Create points that represent the width of the road
            Vector3 v1 = s1 + cross;
            Vector3 v2 = s1 - cross;
            Vector3 v3 = s2 + cross;
            Vector3 v4 = s2 - cross;

            vectors.Add(v1);
            vectors.Add(v2);
            vectors.Add(v3);
            vectors.Add(v4);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);

            int idx1, idx2, idx3, idx4;
            idx4 = vectors.Count - 1;
            idx3 = vectors.Count - 2;
            idx2 = vectors.Count - 3;
            idx1 = vectors.Count - 4;

            // first triangle v1, v3, v2
            indices.Add(idx1);
            indices.Add(idx3);
            indices.Add(idx2);

            // second         v3, v4, v2
            indices.Add(idx3);
            indices.Add(idx4);
            indices.Add(idx2);
        }

        //foreach (var n in way.NodeIDs)
        //{
        //    OsmNode p1 = map.nodes[n];
        //    Vector3 s1 = p1 - origin;
        //    GSDConstruction.CreateNode(road, false, default(Vector3), false, true, s1 + way.LocalPos);
        //}
        //road.UpdateRoad();
    }
}

