using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Rocket.Surgery.Airframe.Data.DuckDuckGo;

namespace Bang.Pages
{
    public class SearchViewModel : ReactiveObject
    {
        private readonly IDuckDuckGoService _duckDuckGoService;
        private string _searchText;
        private IEnumerable<RelatedTopic> _searchResults = Enumerable.Empty<RelatedTopic>();

        public SearchViewModel(IDuckDuckGoService duckDuckGoService)
        {
            _duckDuckGoService = duckDuckGoService;

            this.WhenAnyValue(x => x.SearchText)
                .Do(_ => { })
                .Where(x => string.IsNullOrEmpty(x))
                .Skip(1)
                .Select(_ => Enumerable.Empty<RelatedTopic>())
                .ToObservableChangeSet(x => x.FirstUrl)
                .DefaultIfEmpty()
                .ToCollection()
                .BindTo(this, x => x.SearchResults);

            this.WhenAnyValue(x => x.SearchText)
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Trim())
                .SelectMany(ExecuteSearch)
                .ToCollection()
                .BindTo(this, x => x.SearchResults);
        }

        public IEnumerable<RelatedTopic> SearchResults
        {
            get => _searchResults;
            set => this.RaiseAndSetIfChanged(ref _searchResults, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        private IObservable<IChangeSet<RelatedTopic, string>> ExecuteSearch(string arg) =>
            _duckDuckGoService.Query(arg);
    }
}