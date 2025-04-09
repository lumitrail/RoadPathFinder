using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics.CodeAnalysis;
using RoadPathFinder.Models.Elements;

namespace RoadPathFinder.Agents.AStarIntermediates
{
    /// <summary>
    /// next links to visit
    /// </summary>
    internal class Candidates
    {
        public int Count => _candidatesKeys.Count;

        /// <summary>
        /// key: total heuristic cost(AccumulatedDistance + HeuristicDistance)
        /// </summary>
        private SortedDictionary<double, RouteTreeNode> _candidates { get; } = new();

        /// <summary>
        /// key: directional link ID (negative ID = reverse direction)<br></br>
        /// </summary>
        /// <remarks>value: key of _candidates</remarks>
        private Dictionary<long, double> _candidatesKeys { get; } = new();

        /// <summary>
        /// for lock
        /// </summary>
        private object _addPopLockObj { get; } = new();


        // 되어야 하는 것: 휴리스틱 cost로 정렬(double) -> route tree node
        // 되어야 하는 것: id -> route tree node
        // 이거 2차원인거 같은데
        // 

        public Candidates()
        {
        }


        public void Add(RouteTreeNode newCandidate)
        {
            lock (_addPopLockObj)
            {
                long directionalID = newCandidate.DirectionalLinkID;

                if (_candidatesKeys.TryGetValue(directionalID, out double oldCandidatesKey)
                    && oldCandidatesKey > newCandidate.AccumulatedDistance + newCandidate.HeuristicDistance)
                {
                    // new candidate is better!
                    TryRemoveCandidate(directionalID);
                }

                AddToDictionaries(newCandidate);
            }
        }

        /// <summary>
        /// Get the most promising candidate and get it removed from candidates list
        /// </summary>
        /// <returns></returns>
        public RouteTreeNode? Pop()
        {
            lock (_addPopLockObj)
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkID"></param>
        /// <returns></returns>
        public bool Exists(long linkID, out EDirection existingCases)
        {
            bool forward = _candidatesKeys.ContainsKey(linkID);
            bool backward = _candidatesKeys.ContainsKey(-linkID);

            if (forward && backward)
            {
                existingCases = EDirection.Both;
            }
            else if (forward)
            {
                existingCases = EDirection.Forward;
            }
            else if (backward)
            {
                existingCases = EDirection.Backward;
            }
            else
            {
                existingCases = EDirection.Both;
            }

            return forward || backward;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="candidate"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <remarks>use inside lock</remarks>
        private void AddToDictionaries(RouteTreeNode candidate)
        {
            ArgumentNullException.ThrowIfNull(candidate, nameof(candidate));

            double newKey = candidate.AccumulatedDistance + candidate.HeuristicDistance;

            while (!_candidates.TryAdd(newKey, candidate))
            {
                newKey *= 1.00001;
            }

            _candidatesKeys[candidate.DirectionalLinkID] = newKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directionalID"></param>
        /// <remarks>use inside lock</remarks>
        private bool TryRemoveCandidate(long directionalID)
        {
            return _candidatesKeys.TryGetValue(directionalID, out double candidatesKey)
                && _candidatesKeys.Remove(directionalID)
                && _candidates.Remove(candidatesKey);
        }
    }
}
