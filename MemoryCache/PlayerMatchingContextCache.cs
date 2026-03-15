using JNetwork;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace LearningServer01.MemoryCache
{
    public class PlayerMatchingContextCache
    {
        readonly ConcurrentDictionary<string, List<string>> _playerMatchingExclusionList = new ConcurrentDictionary<string, List<string>>();

        public IReadOnlyList<string> GetMatchingListOrdered(string searcherId)
        {
            _playerMatchingExclusionList.TryGetValue(searcherId, out var res);

            if (res == null)
                return new List<string>();

            return res;
        }

        public void Add(string searcherId, string foundId)
        {
            var list = _playerMatchingExclusionList.GetOrAdd(searcherId, _ => new List<string>());

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == foundId)
                {
                    list.RemoveAt(i);
                    break;
                }
            }

            list.Add(foundId);

            if (list.Count > TEMP_Data.MatchingQueueManagementCount)
                list.RemoveRange(0, list.Count - TEMP_Data.MatchingQueueManagementCount);
        }

        public void Remove(string searcherId)
        {
            _playerMatchingExclusionList.TryRemove(searcherId, out var _);
        }
    }
}
