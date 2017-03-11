using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;
using Assets.Map.Pathfinding;
using Assets.Util;
using Map;
using UnityEngine;

namespace Assets.Game
{
    public class MovableBoardMesh : BoardMesh
    {
        protected int NextPathId { get; set; } = -1;
        protected PathfindingJobInfo CurrentPathInfo { get; set; }

        private const float MovementPerSecond = 5.5f;

        public CubicalCoordinate PreviousPosition { get; set; }
        public CubicalCoordinate Goal { get; set; }

        protected override void Start()
        {
            base.Start();

            PreviousPosition = Position;
        }

        protected override void Update()
        {
            base.Update();

            Position = MapRenderer.WorldToCubicalCoordinate(CreateWorldPos());

            if (Position != Goal)
            {
                if (IsPathValid())
                {
                    // Take step;
                    AdvanceOnPath();
                }
                else
                {
                    // Get new path

                    // If we're not already searching
                    if (NextPathId == -1)
                    {
                        RequestNewPath();
                    }
                    else
                    {
                        // Check on the state of the job
                        if (PathfindingJobManager.Instance.GetInfo(NextPathId).State == JobState.Failure)
                        {
                            // Pathing has failed for some reason, lets try again
                            RequestNewPath();
                        }
                        else if (PathfindingJobManager.Instance.IsFinished(NextPathId))
                        {
                            CurrentPathInfo = PathfindingJobManager.Instance.GetInfo(NextPathId);
                            PathfindingJobManager.Instance.ClearJob(NextPathId);

                            Debug.Log("Path: " + string.Join("-", Enumerable.Range(1, CurrentPathInfo.Path.Count - 1)
                                          .Select(i => (CurrentPathInfo.Path[i].DistanceTo(CurrentPathInfo.Path[i - 1])).ToString())
                                          .ToArray()));


                            NextPathId = -1;
                            AdvanceOnPath();
                        }
                    }
                }
            }
        }

        protected void AdvanceOnPath()
        {
            Vector3 currentPos = CreateWorldPos();

            if (CurrentPathInfo.Path[0] == Position)
            {
                CurrentPathInfo.Path.RemoveAt(0);
                PreviousPosition = Position;

                // Previous position has changed (which is the point based on which we place the mesh in the world)
                // A new draw offset needs to be calculated

                DrawOffset = currentPos - MapRenderer.CubicalCoordinateToWorld(PreviousPosition);
            }

            Vector3 nextPos = Vector3.MoveTowards(
                currentPos,
                MapRenderer.CubicalCoordinateToWorld(CurrentPathInfo.Path[0]),
                MovementPerSecond * Time.deltaTime);

            DrawOffset = nextPos - MapRenderer.CubicalCoordinateToWorld(PreviousPosition);
        }

        protected override void SetWorldPos()
        {
            transform.position = CreateWorldPos();
            MapRenderer.MarkTileSelectedForNextFrame(Position);
        }

        private Vector3 CreateWorldPos()
        {
            return MapRenderer.CubicalCoordinateToWorld(PreviousPosition) + DrawOffset;
        }

        protected void RequestNewPath()
        {
            NextPathId = PathfindingJobManager.Instance.CreateJob(Position, Goal);
        }

        protected bool IsPathValid()
        {
            // TODO: Add further validation

            if (CurrentPathInfo?.GoalPos != Goal)
            {
                return false;
            }

            return true;
        }
    }
}