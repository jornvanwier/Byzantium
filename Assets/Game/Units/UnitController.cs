using UnityEngine;
using System.Collections;
using Assets.Game;
using Assets.Game.Units;
using Assets.Map;
using Assets.Map.Pathfinding;
using Map;
using Assets.Util;

public class UnitController : MonoBehaviour
{
    private UnitBase attachedUnit = null;
    private MapRenderer mapRenderer = null;

    private const float MovementSpeed = 1.5f;
    private const float RotationSpeed = 3.5f;

    private PathfindingJobInfo CurrentPathInfo;
    private Vector3 MovementDrawOffset;
    private CubicalCoordinate Position;
    private CubicalCoordinate PreviousPosition;
    private CubicalCoordinate Goal;
    private int NextPathID = -1;

    public void AttachUnit(UnitBase unit)
    {
        attachedUnit = unit;
    }
    public void AttachMapRenderer(MapRenderer maprenderer)
    {
        mapRenderer = maprenderer;
    }
    public void SetGoal(CubicalCoordinate goal)
    {
        Goal = goal;
    }



    public void Start()
    {
        Position = mapRenderer.WorldToCubicalCoordinate(transform.position);
        PreviousPosition = Position;
    }

    public void Update()
    {
        attachedUnit.Draw();

        SetWorldPosition(CreateWorldPos());

        Position = mapRenderer.WorldToCubicalCoordinate(CreateWorldPos());

        if (Position == Goal)
            return;
        if (IsPathValid())
        {
            AdvanceOnPath();
        }
        else
        {
            if (NextPathID == -1)
            {
                RequestNewPath();
            }
            else
            {
                // Check on the state of the job
                if (PathfindingJobManager.Instance.GetInfo(NextPathID).State == JobState.Failure)
                {
                    // Pathing has failed for some reason, lets try again
                    RequestNewPath();
                }
                else if (PathfindingJobManager.Instance.IsFinished(NextPathID))
                {
                    CurrentPathInfo = PathfindingJobManager.Instance.GetInfo(NextPathID);
                    PathfindingJobManager.Instance.ClearJob(NextPathID);

                    NextPathID = -1;
                    AdvanceOnPath();
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
            MovementDrawOffset = currentPos - mapRenderer.CubicalCoordinateToWorld(PreviousPosition);
        }

        Vector3 nextPos = Vector3.MoveTowards(currentPos, mapRenderer.CubicalCoordinateToWorld(CurrentPathInfo.Path[0]), MovementSpeed * Time.deltaTime);
        MovementDrawOffset = nextPos - mapRenderer.CubicalCoordinateToWorld(PreviousPosition);

        SetWorldRotation(Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(nextPos - mapRenderer.CubicalCoordinateToWorld(CurrentPathInfo.Path[0])),
            Time.deltaTime * RotationSpeed)
        );

    }

    protected void SetUnitWorldPos(Vector3 position)
    {
        attachedUnit.Position = position;
    }
    protected void SetUnitWorldRotation(Quaternion rotation)
    {
        attachedUnit.Rotation = rotation;
    }
    protected void SetWorldPosition(Vector3 position)
    {
        SetUnitWorldPos(position);
        transform.position = position;
    }
    protected void SetWorldRotation(Quaternion rotation)
    {
        SetUnitWorldRotation(rotation);
        transform.rotation = rotation;
    }

    protected Vector3 CreateWorldPos()
    {
        return mapRenderer.CubicalCoordinateToWorld(PreviousPosition) + MovementDrawOffset;
    }
    protected void RequestNewPath()
    {
        NextPathID = PathfindingJobManager.Instance.CreateJob(Position, Goal);
    }
    protected bool IsPathValid()
    {
        if (CurrentPathInfo?.GoalPos != Goal)
        {
            return false;
        }

        return true;
    }


}