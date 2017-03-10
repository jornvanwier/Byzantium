using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
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

        private float lastStepPercentage = 0;
        private const float MovementPerSecond = 1f;

        public CubicalCoordinate PreviousPosition { get; set; }
        public CubicalCoordinate Goal { get; set; }

        protected override void Start()
        {
            base.Start();

            PreviousPosition = Position;
        }

        protected override void Update()
        {

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
                            NextPathId = -1;
                            AdvanceOnPath();
                        }
                    }
                }
            }

            base.Update();
        }

        protected void AdvanceOnPath()
        {
            // Increase the percentage of distance we should be between our current
            lastStepPercentage += Time.deltaTime * MovementPerSecond;
            if (lastStepPercentage >= 1)
            {
                lastStepPercentage = 1;
            }

            Vector3 deltaPoint = Utils.DeltaPointBetween(MapRenderer.CubicalCoordinateToWorld(PreviousPosition),
                MapRenderer.CubicalCoordinateToWorld(CurrentPathInfo.Path.Last()), lastStepPercentage);

            DrawOffset = deltaPoint;

            Debug.Log($"{MapRenderer.CubicalCoordinateToWorld(PreviousPosition)},{MapRenderer.CubicalCoordinateToWorld(CurrentPathInfo.Path.Last())},{DrawOffset}");

            Debug.Log(lastStepPercentage);

            Debug.Log(CurrentPathInfo.Path.Last());
            if (CurrentPathInfo.Path.Last() == Position || lastStepPercentage >= 1)
            {
                CurrentPathInfo.Path.RemoveAt(CurrentPathInfo.Path.Count - 1);
                lastStepPercentage = 0;
                PreviousPosition = Position;
                Debug.Log("reached pos");
            }
        }

        protected override void SetWorldPos()
        {
            transform.position = MapRenderer.CubicalCoordinateToWorld(PreviousPosition) + DrawOffset;
//            Position = MapRenderer.WorldToCubicalCoordinate(transform.position);
            MapRenderer.MarkTileSelectedForNextFrame(Position);
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