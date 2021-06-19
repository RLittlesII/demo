using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Rocket.Surgery.Airframe.Data;

namespace Bang.Pages
{
    public class SearchViewModel : ReactiveObject
    {
        private readonly IDuckDuckGoService _duckDuckGoService;
        private string _searchText;
        private ReadOnlyObservableCollection<DuckDuckGoQueryResult> _searchResults;

        public SearchViewModel(IDuckDuckGoService duckDuckGoService)
        {
            _duckDuckGoService = duckDuckGoService;

            SearchCommand = ReactiveCommand.CreateFromObservable<string, IChangeSet<DuckDuckGoQueryResult, string>>(ExecuteSearch);

            this.WhenAnyValue(x => x.SearchText)
                .WhereNotNull()
                .Select(x => x.Trim())
                .InvokeCommand(this, x => x.SearchCommand);

            SearchCommand
                .Bind(out _searchResults)
                .DisposeMany()
                .Subscribe();
        }

        public ReactiveCommand<string,IChangeSet<DuckDuckGoQueryResult,string>> SearchCommand { get; }

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        private IObservable<IChangeSet<DuckDuckGoQueryResult, string>> ExecuteSearch(string arg) =>
            _duckDuckGoService.Query(arg);
    }
}