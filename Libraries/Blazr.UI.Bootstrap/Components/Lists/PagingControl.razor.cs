/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

public partial class PagingControl
    : ComponentBase, IPagingControl
{
    private int Page = 0;
    private int ListCount = 0;
    private PagingState _pagingState => new PagingState() { PageSize = this.PageSize, StartIndex = this.ReadStartRecord };
    private bool hasPages => LastPage > 0;

    [Parameter] public int PageSize { get; set; } = 5;

    [Parameter] public int BlockSize { get; set; } = 10;

    [Parameter] public Func<PagingState, ValueTask<PagingState>>? PagingProvider { get; set; }

    [Parameter] public bool ShowPageOf { get; set; } = true;

    [CascadingParameter] private ListContext? ListContext { get; set; }

    public async void NotifyListChangedAsync()
        => await GotToPage();

    protected async override Task OnInitializedAsync()
    {
        await this.SetPage();
        if (this.ListContext is not null)
            this.ListContext.PagingReset += this.OnPagingReset;
    }

    private async Task SetPage()
    {
        PagingState state = new();
        if (this.ListContext is not null)
            state = await this.ListContext.SetPage(_pagingState);

        else if (this.PagingProvider is not null)
            state = await PagingProvider(_pagingState);

        this.Page = state.Page;
        this.ListCount = state.ListTotalCount;
    }

    private void OnPagingReset(object? sender, PagingEventArgs e)
    {
        this.Page = e.PagingState.Page;
        this.InvokeAsync(StateHasChanged);
    }

    private int DisplayPage => this.Page + 1;

    private int LastPage => PageSize == 0 || ListCount == 0
        ? 0
        : ((int)Math.Ceiling(Decimal.Divide(this.ListCount, this.PageSize))) - 1;

    private int LastDisplayPage => this.LastPage + 1;

    private int ReadStartRecord => this.Page * this.PageSize;

    private int Block => (int)Math.Floor(Decimal.Divide(this.Page, this.BlockSize));

    private bool AreBlocks => this.ListCount > this.BlockSize * this.PageSize;

    private int BlockStartPage => this.Block * this.BlockSize;

    private int BlockEndPage => this.LastPage > (this.BlockStartPage + (BlockSize)) - 1
        ? (this.BlockStartPage + BlockSize) - 1
        : this.LastPage;

    private int LastBlock => (int)Math.Floor(Decimal.Divide(this.LastPage, this.BlockSize));

    private int LastBlockStartPage => LastBlock * this.BlockSize;

    private async Task GotToPage(int page)
    {
        if (page != this.Page)
        {
            this.Page = page;
            await GotToPage();
        }
    }

    private async Task GotToPage()
    {
        await SetPage();
        this.StateHasChanged();
    }

    private string GetCss(int page)
        => page == this.Page ? "btn-primary" : "btn-secondary";

    private async Task MoveBlock(int block)
    {
        var _page = block switch
        {
            int.MaxValue => this.LastBlockStartPage,
            1 => this.Block + 1 > LastBlock ? LastBlock * this.BlockSize : this.BlockStartPage + BlockSize,
            -1 => this.Block - 1 < 0 ? 0 : this.BlockStartPage - BlockSize,
            _ => 0
        };
        await this.GotToPage(_page);
    }

    private async Task GoToBlock(int block)
        => await this.GotToPage(block * this.PageSize);

    public void Dispose()
    {
        if (this.ListContext is not null)
            this.ListContext.PagingReset -= this.OnPagingReset;
    }
}
