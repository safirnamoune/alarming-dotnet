using App.Core.Models;

namespace App.Core.Abstractions;

public interface IUserKeyQueries
{
    // REQ_Rch_User_Keys
    Task<UserKeys?> GetAsync(string userId, CancellationToken ct = default);

    // REQ_Ajout_User_Keys
    Task InsertAsync(string userId, UserKeys keys, CancellationToken ct = default);
}
