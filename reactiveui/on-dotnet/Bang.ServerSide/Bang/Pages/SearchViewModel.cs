using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using ReactiveUI;
using Rocket.Surgery.Airframe.Data;
using Rocket.Surgery.Airframe.Data.DuckDuckGo;

namespace Bang.Pages
{
    public class SearchViewModel : ReactiveObject
    {
        private readonly IDuckDuckGoService _duckDuckGoService;
        private string _searchText;
        private ReadOnlyObservableCollection<RelatedTopic> _searchResults;

        public SearchViewModel(IDuckDuckGoService duckDuckGoService)
        {
            _duckDuckGoService = duckDuckGoService;

            this.WhenAnyValue(x => x.SearchText)
                .WhereNotNull()
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(x => x.Trim())
                .SelectMany(ExecuteSearch)
                .Transform(x => x)
                .Bind(out _searchResults)
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(this, x => x.ChangeState);

            ChangeState = ReactiveCommand.Create<IChangeSet<RelatedTopic,string>>(_ => { });
        }

        public ReactiveCommand<IChangeSet<RelatedTopic,string>, Unit> ChangeState { get; set; }

        public ReadOnlyObservableCollection<RelatedTopic> SearchResults => _searchResults;

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        private IObservable<IChangeSet<RelatedTopic, string>> ExecuteSearch(string arg) =>
            _duckDuckGoService.Query(arg);
    }
}