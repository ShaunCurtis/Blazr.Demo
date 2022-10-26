/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class PagingControlBase<TRecord>
    : UIHtmlComponentBase, IDisposable
    where TRecord : class, new()
{
    [Parameter] public int BlockSize { get; set; } = 10;

    [Parameter] public bool ShowPageOf { get; set; } = true;

    [CascadingParameter] private ListContext<TRecord>? ListContext { get; set; }

    protected override ValueTask<bool> OnParametersChangedAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (ListContext is null)
                throw new NullReferenceException($"No cascaded {nameof(ListContext)} found.");

            this.SetPage();

            this.ListContext.PagingReset += this.OnPagingReset;
            this.ListContext.StateChanged += this.OnStateChanged;
            this.ListContext.ListChanged += this.OnListChanged;
        }
        return ValueTask.FromResult(true);
    }

    protected int Page
        => this.ListContext?.ListState.Page ?? 0;

    protected int ListCount
        => this.ListContext?.ListState.ListTotalCount ?? 0;

    private int PageSize
        => this.ListContext?.ListState.PageSize ?? 10;

    protected bool hasPages
        => LastPage > 0;

    protected int DisplayPage
        => this.Page + 1;

    protected int LastPage
        => PageSize == 0 || ListCount == 0
            ? 0
            : ((int)Math.Ceiling(Decimal.Divide(this.ListCount, this.PageSize))) - 1;

    protected int LastDisplayPage
        => this.LastPage + 1;

    protected int ReadStartRecord
        => this.Page * this.PageSize;

    protected int Block
        => (int)Math.Floor(Decimal.Divide(this.Page, this.BlockSize));

    protected bool AreBlocks
        => this.ListCount > this.BlockSize * this.PageSize;

    protected int BlockStartPage
        => this.Block * this.BlockSize;

    protected int BlockEndPage
        => this.LastPage > (this.BlockStartPage + (BlockSize)) - 1
            ? (this.BlockStartPage + BlockSize) - 1
            : this.LastPage;

    protected int LastBlock
        => (int)Math.Floor(Decimal.Divide(this.LastPage, this.BlockSize));

    protected int LastBlockStartPage
        => LastBlock * this.BlockSize;

    protected void SetPage(PagingRequest? request = null)
    {
        if (this.ListContext is not null)
            this.ListContext.NotifyPagingRequested(this, request);
    }

    protected void OnPagingReset(object? sender, EventArgs e)
        => this.StateHasChanged();

    protected void OnStateChanged(object? sender, ListState<TRecord> listState)
        => this.StateHasChanged();

    protected void OnListChanged(object? sender, EventArgs e)
        => SetPage();

    protected void GotToPage(int page)
    {
        if (page != this.Page)
        {
            SetPage(this.GetPagingRequest(page));
            // TODO - is this right?
            this.StateHasChanged();
        }
    }

    protected string GetCss(int page)
        => page == this.Page ? "btn-primary" : "btn-secondary";

    protected void MoveBlock(int block)
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

    protected void GoToBlock(int block)
        => this.GotToPage(block * this.PageSize);

    protected PagingRequest GetPagingRequest(int page)
        => new PagingRequest { PageSize = this.PageSize, StartIndex = this.PageSize * page };

    public void Dispose()
    {
        if (this.ListContext is null)
            return;

        this.ListContext.PagingReset -= this.OnPagingReset;
        this.ListContext.StateChanged -= this.OnStateChanged;
        this.ListContext.ListChanged -= this.OnListChanged;
    }
}
