﻿using UnityEngine;
using System.Collections;
using Assets.Game;
using Assets.Game.Units;
using Assets.Map;
using Assets.Map.Pathfinding;
using Map;
using Assets.Util;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
    private UnitBase attachedUnit = null;
    private MapRenderer mapRenderer = null;

    private const float MovementSpeed = 1.5f;
    private const float RotationSpeed = 3.5f;

    public CubicalCoordinate Position { get; set; }
    public CubicalCoordinate Goal { get; set; }

    private PathfindingJobInfo currentPathInfo;
    private Vector3 movementDrawOffset;
    private CubicalCoordinate previousPosition;
    private int nextPathID = -1;

    public void AttachUnit(UnitBase unit)
    {
        attachedUnit = unit;
    }
    public void AttachMapRenderer(MapRenderer maprenderer)
    {
        mapRenderer = maprenderer;
    }

    public void Start()
    {
        Position = mapRenderer.WorldToCubicalCoordinate(transform.position);
        previousPosition = Position;
    }

    public void Update()
    {
        attachedUnit.Draw();

        if (currentPathInfo != null && currentPathInfo.Path != null)
        {
            foreach (CubicalCoordinate c in currentPathInfo.Path)
            {
                mapRenderer.MarkTileSelectedForNextFrame(c);
            }
        }

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
            if (nextPathID == -1)
            {
                RequestNewPath();
            }
            else
            {
                // Check on the state of the job
                if (PathfindingJobManager.Instance.GetInfo(nextPathID).State == JobState.Failure)
                {
                    // Pathing has failed for some reason, lets try again
                    RequestNewPath();
                }
                else if (PathfindingJobManager.Instance.IsFinished(nextPathID))
                {
                    currentPathInfo = PathfindingJobManager.Instance.GetInfo(nextPathID);
                    PathfindingJobManager.Instance.ClearJob(nextPathID);

                    nextPathID = -1;
                    AdvanceOnPath();
                }
            }
        }


        
        
    }

    protected void AdvanceOnPath()
    {
        Vector3 currentPos = CreateWorldPos();

        if (currentPathInfo.Path[0] == Position)
        {
            currentPathInfo.Path.RemoveAt(0);
            previousPosition = Position;
            movementDrawOffset = currentPos - mapRenderer.CubicalCoordinateToWorld(previousPosition);
        }

        Vector3 nextPos = Vector3.MoveTowards(currentPos, mapRenderer.CubicalCoordinateToWorld(currentPathInfo.Path[0]), attachedUnit.WalkSpeed() * Time.deltaTime);
        movementDrawOffset = nextPos - mapRenderer.CubicalCoordinateToWorld(previousPosition);

        SetWorldRotation(Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(nextPos - mapRenderer.CubicalCoordinateToWorld(currentPathInfo.Path[0])),
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
        return mapRenderer.CubicalCoordinateToWorld(previousPosition) + movementDrawOffset;
    }
    protected void RequestNewPath()
    {
        nextPathID = PathfindingJobManager.Instance.CreateJob(Position, Goal);
    }
    protected bool IsPathValid()
    {
        if (currentPathInfo?.GoalPos != Goal)
        {
            return false;
        }

        return true;
    }


}