using ActorDemo.Model.Resources;
using SimulationCore.HCCMElements;

namespace ActorDemo.Model.Requests
{
    public class SearchRequest(string activity, Entity searcher, DateTime time, EquipmentType search)
        : ActivityRequest(activity, [searcher], time)
    {
        public const string SEARCH_ACTIVITY = "SearchActivity";

        // Assumption for scenario 1: only searches for single production equipment type
        // no FORs and matchmaking yet
        public EquipmentType Search { get; } = search;
    }
}