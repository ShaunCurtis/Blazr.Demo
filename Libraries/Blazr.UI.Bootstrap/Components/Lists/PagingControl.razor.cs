/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

public partial class PagingControl
    : ComponentBase, IPagingControl
{
    private int Page => this.ListContext?.ListState.Page ?? 0;
    private int ListCount => this.ListContext?.ListState.ListTotalCount ?? 0 ;
    private int PageSize => this.ListContext?.ListState.PageSize ?? 10 ;
    private bool hasPages => LastPage > 0;

    [Parameter] public int BlockSize { get; set; } = 10;

    [Parameter] public bool ShowPageOf { get; set; } = true;

    [CascadingParameter] private ListContext? ListContext { get; set; }

    public async void NotifyListChangedAsync()
        => await SetPage();

    protected async override Task OnInitializedAsync()
    {
        if (ListContext is null)
            throw new NullReferenceException($"No cascaded {nameof(ListContext)} found.");

        await this.SetPage();
        
        if (this.ListContext is not null)
            this.ListContext.PagingReset += this.OnPagingReset;
    }

    private async Task SetPage(int? page = null)
    {
        if (this.ListContext is not null)
            await this.ListContext.GoToPage(page);
    }

    private void OnPagingReset(object? sender, PagingEventArgs e)
    {
        this.ListContext?.ListState.Set(e.PagingState.Page);
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
            await SetPage(page);
            this.StateHasChanged();
        }
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
