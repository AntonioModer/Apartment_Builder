﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Foxsys.ApartmentEditor
{
    public class Apartment : ScriptableObject
    {
        #region factory
        public static Apartment CreateOrGet(string name)
        {
            var fullpath = Path.Combine(PathsConfig.Instance.PathToApartments, name + ".asset");
            Apartment apartment = AssetDatabase.LoadAssetAtPath<Apartment>(fullpath);

            if (apartment == null)
            {
                apartment = CreateInstance<Apartment>();
                apartment._Rooms = new List<Room>();
                apartment.Dimensions = new Rect(-Vector2.one * 500, Vector2.one * 1000);
                AssetDatabase.CreateAsset(apartment, fullpath);
            }
            return apartment;
        }
        #endregion

        #region fields

        public float Height;

        public Material WallMaterial;
        public Material FloorMaterial;

        public bool IsGenerateOutside;

        [SerializeField]
        private List<Room> _Rooms;
        [SerializeField]
        private Rect _Dimensions;

        private Vector2[] _DimensionsPoints = new Vector2[4];

        private Room _SelectedRoom;

        #endregion

        #region properties
        public List<Room> Rooms
        {
            get
            {
                return _Rooms;
            }
        }

        public float Square
        {
            get
            {
                float result = 0;
                foreach (Room room in _Rooms)
                {
                    result += room.Square;
                }
                return result;
            }
        }

        public Rect Dimensions
        {
            get { return _Dimensions; }
            set
            {
                _Dimensions = value;
                _DimensionsPoints[0] = new Vector3(Dimensions.width / 2, Dimensions.height / 2);
                _DimensionsPoints[1] = new Vector3(-Dimensions.width / 2, Dimensions.height / 2);
                _DimensionsPoints[2] = new Vector3(-Dimensions.width / 2, -Dimensions.height / 2);
                _DimensionsPoints[3] = new Vector3(Dimensions.width / 2, -Dimensions.height / 2);
            }
        }

        public Room SelectedRoom
        {
            get { return _SelectedRoom; }
            set
            {
                _SelectedRoom = value;
                int index = _Rooms.FindIndex(x => x == _SelectedRoom);
                if (index > 0)
                {
                    var room = _Rooms[index];
                    _Rooms[index] = _Rooms[0];
                    _Rooms[0] = room;
                }
            }
        }

        #endregion

        public void Draw(Grid grid)
        {
            DrawDimensions(grid);
            foreach (Room room in _Rooms)
            {
                room.Draw(grid);
            }
        }
        private void DrawDimensions(Grid grid)
        {
            Handles.color = Color.green;
            for(int i = 0; i < _DimensionsPoints.Length; i++)
                Handles.DrawLine(
                    grid.GridToGUI((_DimensionsPoints[i])),
                    grid.GridToGUI(_DimensionsPoints[(i + 1) % _DimensionsPoints.Length]));
        }

        public Vector2 GetPointProjectionOnNearestContour(Vector2 point)
        {
            Vector2 result = point; 
            float minDistance = float.MaxValue;
            foreach (var room in Rooms)
            {
                if(room == SelectedRoom) continue;

                foreach (var wall in room.Walls)
                {
                    var distance = MathUtils.DistanceFromPointToLine(point, wall.Begin, wall.End);
                    if (distance < minDistance)
                    {
                        var projection = MathUtils.PointProjectionToOnLine(point, wall.Begin, wall.End);
                        if (wall.IsPointOnWall(point))
                        {
                            result = projection;
                            minDistance = distance;
                        }
                    }
                }
            }
            for (int i = 0; i < _DimensionsPoints.Length; i++)
            {
                Vector2 begin = _DimensionsPoints[i], end = _DimensionsPoints[(i + 1) % _DimensionsPoints.Length];
                var distance = MathUtils.DistanceFromPointToLine(point, begin, end);
                if (distance < minDistance)
                {
                    result = MathUtils.PointProjectionToOnLine(point, begin, end);
                    minDistance = distance;
                }
            }
            return result;
        }

        public List<Wall> GetWallsWithPoint(Vector2 point)
        {
            List<Wall> result = new List<Wall>();
            foreach (var room in Rooms)
            {
                foreach (Wall wall in room.Walls)
                {
                    if(wall.IsPointOnWall(point))
                        result.Add(wall);
                }
            }
            return result;
        }
        public Room PointInsideRooms(Vector2 point, Room excludeRoom = null)
        {
            foreach (var room in _Rooms)
            {
                if(excludeRoom != room && room.IsPointInside(point))
                {
                    return room;
                }
            }
            return null;
        }

        public bool IsApartmentInRect(Rect rect)
        {
            foreach (var room in _Rooms)
            {
                if (!room.IsInsideRect(rect))
                    return false;
            }
            return true;
        }
    }
}
