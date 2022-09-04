/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

public partial class PagingControl
    : UIComponent, IPagingControl
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

    public void NotifyListChanged()
        => SetPage();

    protected override Task OnParametersChangedAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (ListContext is null)
                throw new NullReferenceException($"No cascaded {nameof(ListContext)} found.");

            this.SetPage();

            if (this.ListContext is not null)
                this.ListContext.PagingReset += this.OnPagingReset;
        }
        return Task.CompletedTask;
    }

    private void SetPage(PagingRequest? request = null)
    {
        if (this.ListContext is not null)
            this.ListContext.NotifyPagingRequested(this, request);
    }

    private void OnPagingReset(object? sender, EventArgs e)
        => this.StateHasChanged();

    private void GotToPage(int page)
    {
        if (page != this.Page)
        {
            SetPage(this.GetPagingRequest(page));
            // TODO - is this right?
            this.StateHasChanged();
        }
    }

    private string GetCss(int page)
        => page == this.Page ? "btn-primary" : "btn-secondary";

    private void MoveBlock(int block)
    {
        var _page = block switch
        {
            int.MaxValue => this.LastBlockStartPage,
            1 => this.Block + 1 > LastBlock ? LastBlock * this.BlockSize : this.BlockStartPage + BlockSize,
            -1 => this.Block - 1 < 0 ? 0 : this.BlockStartPage - BlockSize,
            _ => 0
        };
        this.GotToPage(_page);
    }

    private void GoToBlock(int block)
        => this.GotToPage(block * this.PageSize);

    private PagingRequest GetPagingRequest(int page)
        => new PagingRequest { PageSize = this.PageSize, StartIndex = this.PageSize * page };

    public void Dispose()
    {
        if (this.ListContext is not null)
            this.ListContext.PagingReset -= this.OnPagingReset;
    }
}
