using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace Bang.Pages
{
    public partial class CardView
    {
        public CardView()
        {
            
            this.WhenAnyValue(x => x.ViewModel.News)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .Subscribe(_ => StateHasChanged());

            this.WhenAnyValue(x => x.ViewModel.InitializeCommand)
                .WhereNotNull()
                .Select(_ => Unit.Default)
                .InvokeCommand(this, x => x.ViewModel.InitializeCommand);
        }

        protected override void OnInitialized()
        {
            ViewModel = _cardViewModel;
        }
    }
}