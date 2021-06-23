using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Rocket.Surgery.Airframe.Data;
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

            var empty =
                this.WhenAnyValue(x => x.SearchText)
                    .Where(string.IsNullOrEmpty)
                    .Skip(1)
                    .Select(_ => Enumerable.Empty<RelatedTopic>())
                    .ToObservableChangeSet(x => x.FirstUrl);

            this.WhenAnyValue(x => x.SearchText)
                .WhereNotNull()
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(x => x.Trim())
                .SelectMany(ExecuteSearch)
                .Merge(empty)
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