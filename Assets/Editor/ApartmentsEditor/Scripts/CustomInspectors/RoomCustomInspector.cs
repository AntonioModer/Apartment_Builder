﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    [CustomEditor(typeof(Room))]
    public class RoomCustomInspector : Editor
    {
        private Room _ThisRoom;
        public void OnEnable()
        {
            _ThisRoom = (Room) target;
        }
        public override void OnInspectorGUI()
        {
            _ThisRoom.ContourColor = EditorGUILayout.ColorField(_ThisRoom.ContourColor);
            DrawContourPositions();
        }

        public void DrawContourPositions()
        {
            var dimension = _ThisRoom.ParentApartment.Dimensions;
            for (int i = 0; i < _ThisRoom.Walls.Count; i++)
            {
                var newPosition = EditorGUILayout.Vector2Field("Point " + i,
                    _ThisRoom.Walls[i].End);

                if (dimension.Contains(newPosition))
                {
                    _ThisRoom.Walls[i].End = newPosition;
                    _ThisRoom.Walls[(i + 1) % _ThisRoom.Walls.Count].Begin = _ThisRoom.Walls[i].End;
                }

            }
        }
    }
}