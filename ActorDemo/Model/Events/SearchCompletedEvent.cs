using ActorDemo.Model.Entities;
using ActorDemo.Model.Requests;
using ActorDemo.Model.Resources;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace ActorDemo.Model.Events
{
    public class SearchCompletedEvent(ControlUnit parentControlUnit, FactoryOwner searcher, IList<ProductionEquipment> searchResult)
        : Event(EventType.Standalone, parentControlUnit)
    {
        public override Entity[] AffectedEntities => [Searcher];

        public FactoryOwner Searcher { get; } = searcher;

        public IList<ProductionEquipment> SearchResult { get; } = searchResult;

        public override Event Clone() => new SearchCompletedEvent(ParentControlUnit, Searcher, SearchResult);

        public override string ToString() => nameof(SearchCompletedEvent);

        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            // Assumption: Searcher (= buyer) will simply buy the first equipment in the search result

            ProductionEquipment equiment = SearchResult.First();

            ParentControlUnit.AddRequest(
                new BuyRequest(
                    BuyRequest.BUY_ACTIVITY,
                    Searcher,
                    time,
                    equiment.Owner,
                    [equiment]
                )
            );
        }
    }
}