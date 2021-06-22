using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
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

            SearchCommand = ReactiveCommand.CreateFromObservable<string, IChangeSet<RelatedTopic, string>>(ExecuteSearch);

            var searchTextChanged =
                this.WhenAnyValue(x => x.SearchText)
                    .Publish()
                    .RefCount();

            searchTextChanged
                .WhereNotNull()
                .Select(x => x.Trim())
                .Merge(searchTextChanged
                    .Where(string.IsNullOrWhiteSpace)
                    .Skip(1))
                .InvokeCommand(this, x => x.SearchCommand);

            SearchCommand
                .Bind(out _searchResults)
                .DisposeMany()
                .Subscribe();

            SearchCommand
                .Transform(x => x)
                .Subscribe();
        }

        public ReadOnlyObservableCollection<RelatedTopic> SearchResults => _searchResults;

        public ReactiveCommand<string,IChangeSet<RelatedTopic,string>> SearchCommand { get; }

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        private IObservable<IChangeSet<RelatedTopic, string>> ExecuteSearch(string arg) =>
            _duckDuckGoService.Query(arg, string.IsNullOrEmpty(arg));
    }
}