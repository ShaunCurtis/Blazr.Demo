/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

public partial class PagingControl
    : ComponentBase, IPagingControl
{
    private int Page => this.ListContext?.Page ?? 0;
    private int ListCount => this.ListContext?.ListTotalCount ?? 0;
    private int PageSize => this.ListContext?.PageSize ?? 10;
    private bool hasPages => LastPage > 0;
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

    [Parameter] public int BlockSize { get; set; } = 10;

    [Parameter] public bool ShowPageOf { get; set; } = true;

    [CascadingParameter] private ListContext? ListContext { get; set; }

    public async void NotifyListChangedAsync()
        => await SetPageAsync();

    protected async override Task OnInitializedAsync()
    {
        if (ListContext is null)
            throw new NullReferenceException($"No cascaded {nameof(ListContext)} found.");

        await this.SetPageAsync();

        if (this.ListContext is not null)
            this.ListContext.PagingReset += this.OnPagingReset;
    }

    private async Task SetPageAsync(PagingRequest? request = null)
    {
        if (this.ListContext is not null)
            await this.ListContext.PageAsync(request);
    }

    private void OnPagingReset(object? sender, PagingEventArgs e)
    {
        this.ListContext?.Set(e.Request);
        this.InvokeAsync(StateHasChanged);
    }

    private async Task GotToPageAsync(int page)
    {
        if (page != this.Page)
        {
            await SetPageAsync(this.GetPagingRequest(page));
            this.StateHasChanged();
        }
    }

    private string GetCss(int page)
        => page == this.Page ? "btn-primary" : "btn-secondary";

    private async Task MoveBlockAsync(int block)
    {
        var _page = block switch
        {
            int.MaxValue => this.LastBlockStartPage,
            1 => this.Block + 1 > LastBlock ? LastBlock * this.BlockSize : this.BlockStartPage + BlockSize,
            -1 => this.Block - 1 < 0 ? 0 : this.BlockStartPage - BlockSize,
            _ => 0
        };
        await this.GotToPageAsync(_page);
    }

    private async Task GoToBlockAsync(int block)
        => await this.GotToPageAsync(block * this.PageSize);

    private PagingRequest GetPagingRequest(int page)
        =>  new PagingRequest { PageSize = this.PageSize, StartIndex = this.PageSize * page };

    public void Dispose()
    {
        if (this.ListContext is not null)
            this.ListContext.PagingReset -= this.OnPagingReset;
    }
}
