using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Map.Pathfinding
{
    public class PathfindingJobInfo
    {
        private static int _lastId;

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

    public static class PathfindingJobManager
    {
        private static readonly Dictionary<int, PathfindingJobInfo> Storage = new Dictionary<int, PathfindingJobInfo>();

        private static Thread Worker { get; set; }

        private static Queue<PathfindingJobInfo> WorkQueue { get; } = new Queue<PathfindingJobInfo>();

        public static HexBoard Map { get; set; }

        public static void Init(HexBoard map)
        {
            Map = map;
            Worker = new Thread(DoWork);
            Worker.Start();
        }

        public static void DoWork()
        {
            for (;;)
            {
                if (WorkQueue.Count < 1)
                {
                    // Replace with less retarded thing
                    Thread.Sleep(100);
                    continue;
                }

                PathfindingJobInfo job;
                lock (WorkQueue)
                {
                    job = WorkQueue.Dequeue();
                }

                PathfindBetween(job);
            }
        }

        public static int CreateJob(CubicalCoordinate start, CubicalCoordinate goal)
        {
            var jobInfo = new PathfindingJobInfo
            {
                StartPos = start,
                GoalPos = goal
            };

            if (Storage.ContainsKey(jobInfo.Id))
            {
                Debug.LogError("Duplicate job id");
                return -1;
            }

            Storage.Add(jobInfo.Id, jobInfo);

            lock (WorkQueue)
            {
                WorkQueue.Enqueue(jobInfo);
            }

            return jobInfo.Id;
        }

        public static bool JobExists(int id)
        {
            return Storage.ContainsKey(id);
        }

        public static bool IsFinished(int id)
        {
            if (Storage.ContainsKey(id))
                return Storage[id].State != JobState.Working;
            return false;
        }

        public static void ClearJob(int id)
        {
            Storage.Remove(id);
        }

        public static PathfindingJobInfo GetInfo(int id)
        {
            return Storage.ContainsKey(id) ? Storage[id] : null;
        }

        public static void PathfindBetween(object state)
        {
            Utils.LogOperationTime("pathfind", () =>
            {
                var info = (PathfindingJobInfo) state;
                info.Path = Map?.FindPath(info.StartPos, info.GoalPos);
                info.State = info.Path == null ? JobState.Failure : JobState.Success;
            });
        }
    }
}