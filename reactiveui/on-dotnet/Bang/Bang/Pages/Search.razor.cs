using System;
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
            this.WhenAnyValue(x => x.SearchViewModel.SearchResults)
                .Subscribe(_ => InvokeAsync(StateHasChanged));
        }

        protected override void OnInitialized()
        {
            ViewModel = SearchViewModel;
        }
    }
}