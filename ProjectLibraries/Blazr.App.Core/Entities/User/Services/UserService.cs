/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class UserService
    : BaseEntityService<UserEntity>
{
    public readonly SortedDictionary<Guid, string> UserList = new SortedDictionary<Guid, string>();
    private ICQSDataBroker _dataBroker;

    public UserService(ICQSDataBroker cQSDataBroker)
    {
        _dataBroker = cQSDataBroker;
    }

    public async Task EnsureUsersAsync(bool reload = false)
    {
        if (reload || UserList.Count == 0)
            await GetUsersAsync();
    }

    private async Task GetUsersAsync()
    {
        UserList.Clear();
        var query = new ListQuery<DboUser>(new ListProviderRequest<DboUser>());
        var result = await _dataBroker.ExecuteAsync(query);
        if (result.Success)
        {
            foreach (var item in result.Items)
                UserList.Add(item.Id, item.Name);
        }
    }

    public async Task<ClaimsPrincipal> GetUserAsync(Guid Id)
    {
        await EnsureUsersAsync();
        var query = new RecordQuery<DboUser>(Id);
        var result = await _dataBroker.ExecuteAsync(query);
        if (result.Success && result.Record is not null && result.Record.Id.IsNotNull())
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, result.Record.Id.ToString()),
                    new Claim(ClaimTypes.Name, result.Record.Name),
                    new Claim(ClaimTypes.Role, result.Record.Role)
                }, "Test authentication type"));

        return new ClaimsPrincipal(new ClaimsIdentity(new Claim[0], null));
    }
}
