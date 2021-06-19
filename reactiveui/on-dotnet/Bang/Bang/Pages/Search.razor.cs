using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace Bang.Pages
{
    public partial class Search
    {
        protected override void OnInitialized()
        {
            ViewModel = SearchViewModel;
        }
    }
}