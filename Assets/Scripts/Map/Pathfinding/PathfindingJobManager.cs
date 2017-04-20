using System.Collections.Generic;
using System.IO;
using System.Threading;
using Assets.Scripts.Util;
using JetBrains.Annotations;

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

        public static HexBoard Map { get; set; }

        public static int CreateJob(CubicalCoordinate start, CubicalCoordinate goal)
        {
            var jobInfo = new PathfindingJobInfo
            {
                StartPos = start,
                GoalPos = goal
            };

            ThreadPool.QueueUserWorkItem(PathfindBetween, jobInfo);

            Storage.Add(jobInfo.Id, jobInfo);
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