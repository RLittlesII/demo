using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;

namespace Bang.Pages
{
    public partial class Search
    {
        public Search()
        {
            this.WhenAnyObservable(x => x.ViewModel.Changed)
                .Do(_ => StateHasChanged())
                .Subscribe();
        }

        protected override void OnInitialized()
        {
            ViewModel = _searchViewModel;
        }
    }
}