/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface IContextEditService<TEditContext, TRecord>
    where TEditContext : class, IRecordEditContext<TRecord>, IEditContext, new()
    where TRecord : class, new()
{
    public IRecordEditContext<TRecord> EditModel { get; }

    public string? Message { get; }

    public void SetServices(IServiceProvider services);

    public ValueTask<bool> LoadRecordAsync(Guid Id);

    public ValueTask<CommandResult> AddRecordAsync();

    public ValueTask<CommandResult> UpdateRecordAsync();

    public ValueTask<CommandResult> DeleteRecordAsync();
}
