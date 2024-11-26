using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RoadPathFinder.Agents.AStarIntermediates
{
    /// <summary>
    /// A* links to search next
    /// </summary>
    /// <remarks>
    /// not thread-safe
    /// </remarks>
    internal class Candidates
    {
        public int Count => _candidates.Count;

        /// <summary>
        /// key: total heuristic cost(AccumulatedDistance + HeuristicDistance)
        /// </summary>
        private SortedDictionary<double, RouteTreeNode> _candidates { get; } = new();

        /// <summary>
        /// key: directional link ID<br></br>
        /// value: key of _candidates
        /// </summary>
        private Dictionary<long, double> _candidatesKeys { get; } = new();


        public Candidates()
        {
        }


        public void Add(RouteTreeNode newCandidate)
        {
            long directionalID = newCandidate.DirectionalLinkID;

            if (_candidatesKeys.TryGetValue(directionalID, out double oldCandidatesKey)
                && oldCandidatesKey > newCandidate.AccumulatedDistance + newCandidate.HeuristicDistance)
            {
                // new candidate is better!
                RemoveCandidate(directionalID);
            }

            AddToDictionaries(newCandidate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="candidate"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
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
        private void RemoveCandidate(long directionalID)
        {
            try
            {
                double _candidatesKey = _candidatesKeys[directionalID];
                _candidatesKeys.Remove(directionalID);
                _candidates.Remove(_candidatesKey);
            }
            catch
            {
            }
        }
    }
}
