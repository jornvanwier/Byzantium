using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Assets.Util;
using Map;
using UnityEngine;

namespace Assets.Map.Pathfinding
{
    public class PathfindingJobInfo
    {
        private static int _lastId = 0;

        private int id;

        public PathfindingJobInfo()
        {
            Id = ++_lastId;
            State = JobState.Working;
        }

        public JobState State { get; set; }
        public CubicalCoordinate StartPos { get; set; }
        public CubicalCoordinate GoalPos { get; set; }
        public List<CubicalCoordinate> Path { get; set; }

        public int Id { get; }
    }

    public class PathfindingJobManager : Singleton<PathfindingJobManager>
    {
        public HexBoard Map { get; set; }
        private readonly Dictionary<int, PathfindingJobInfo> storage;

        public PathfindingJobManager()
        {
            storage = new Dictionary<int, PathfindingJobInfo>();
        }

        public int CreateJob(CubicalCoordinate start, CubicalCoordinate goal)
        {
            var jobInfo = new PathfindingJobInfo()
            {
                StartPos = start,
                GoalPos = goal
            };
            ThreadPool.QueueUserWorkItem(PathfindBetween, jobInfo);

            storage.Add(jobInfo.Id, jobInfo);
            return jobInfo.Id;
        }

        public bool JobExists(int id)
        {
            return storage.ContainsKey(id);
        }

        public bool IsFinished(int id)
        {
            if (storage.ContainsKey(id))
            {
                return storage[id].State != JobState.Working;
            }
            return false;
        }

        public PathfindingJobInfo GetInfo(int id)
        {
            return storage[id];
        }

        public void PathfindBetween(object state)
        {
            var info = (PathfindingJobInfo) state;
            info.Path = Map.FindPath(info.StartPos, info.GoalPos);
            info.State = info.Path == null ? JobState.Failure : JobState.Success;
        }
    }
}