using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace Bang.Pages
{
    public partial class Search
    {
        public Search()
        {
            this.WhenAnyValue(x => x._searchViewModel.)
                .Subscribe()
            this.WhenAnyValue(x => x._searchViewModel.SearchResults)
                .Where(x => x != null)
                .Select(async _ => await InvokeAsync(StateHasChanged))
                .Subscribe();

            this.WhenAnyObservable(x => x.ViewModel.ChangeState)
                .Select(async _ => await InvokeAsync(StateHasChanged))
                .Subscribe();
        }

        protected override void OnInitialized()
        {
            ViewModel = _searchViewModel;
        }
    }
}