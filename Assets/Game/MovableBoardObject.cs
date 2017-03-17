using Assets.Map.Pathfinding;
using Assets.Util;
using Map;
using UnityEngine;

namespace Assets.Game
{
    public class MovableBoardObject : BoardObject
    {
        protected int NextPathId { get; set; } = -1;
        protected PathfindingJobInfo CurrentPathInfo { get; set; }
        private Vector3 MovementDrawOffset { get; set; }

        private const float MovementPerSecond = 1.5f;
        private const float RotationSpeed = 3.5f;

        public CubicalCoordinate PreviousPosition { get; set; }
        public CubicalCoordinate Goal { get; set; }

        protected override void Start()
        {
            base.Start();

            PreviousPosition = Position;
        }

        protected override void Update()
        {
            SetWorldPos(CreateWorldPos());

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

                MovementDrawOffset = currentPos - MapRenderer.CubicalCoordinateToWorld(PreviousPosition);
            }

            Vector3 nextPos = Vector3.MoveTowards(
                currentPos,
                MapRenderer.CubicalCoordinateToWorld(CurrentPathInfo.Path[0]),
                MovementPerSecond * Time.deltaTime);

            MovementDrawOffset = nextPos - MapRenderer.CubicalCoordinateToWorld(PreviousPosition);

            SetWorldRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextPos - MapRenderer.CubicalCoordinateToWorld(CurrentPathInfo.Path[0])), Time.deltaTime * RotationSpeed));
        }

        protected override void SetWorldPos(Vector3 worldPos)
        {
            transform.position = worldPos;
            MapRenderer.MarkTileSelectedForNextFrame(Position);
        }

        protected override void SetWorldRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        protected Vector3 CreateWorldPos()
        {
            return MapRenderer.CubicalCoordinateToWorld(PreviousPosition) + MovementDrawOffset + DrawOffset;
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