/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface IContextEditService<TEditContext>
    where TEditContext : class, new()
{
    public TEditContext? EditModel { get; }

    public string? Message { get; }

    public void SetServices(IServiceProvider services);

    public ValueTask<bool> LoadRecordAsync(Guid Id);

    public ValueTask<bool> AddRecordAsync();

    public ValueTask<bool> UpdateRecordAsync();

    public ValueTask<bool> DeleteRecordAsync();
}
