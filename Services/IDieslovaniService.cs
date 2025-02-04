    using Diesel_modular_application.Models; // Add the namespace for TableOdstavky, HandleOdstavkyDieslovaniResult, and TableDieslovani
    using Microsoft.AspNetCore.Identity;
    using static Diesel_modular_application.Services.OdstavkyService; // Add the namespace for IdentityUser

    public interface IDieslovaniService
    {
        Task<HandleOdstavkyDieslovaniResult> HandleOdstavkyDieslovani(TableOdstavky? newOdstavka, HandleOdstavkyDieslovaniResult result);
        Task<(bool Success, string Message)> VstupAsync(int idDieslovani);
        Task<(bool Success, string Message)> OdchodAsync(int idDieslovani);
        Task<(bool Success, string Message)> TemporaryLeaveAsync(int idDieslovani);
        Task<(bool Success, string Message, string? TempMessage)> TakeAsync(int idDieslovani, IdentityUser currentUser);
        Task<TableDieslovani?> DetailDieslovaniAsync(int id);
        Task<object> DetailDieslovaniJsonAsync(int id);
        Task<(bool Success, string Message)> DeleteDieslovaniAsync(int iDdieslovani);
        Task<(int totalRecords, List<object> data)> GetTableDataRunningTableAsync(IdentityUser? currentUser, bool isEngineer);
        Task<List<object>> GetTableDataOdDetailOdstavkyAsync(int idodstavky);
        Task<(int totalRecords, List<object> data)> GetTableDataAllTableAsync(IdentityUser? currentUser, bool isEngineer);
        Task<(int totalRecords, List<object> data)> GetTableDatathrashTableAsync(IdentityUser? currentUser, bool isEngineer);
        Task<(int totalRecords, List<object> data)> GetTableUpcomingTableAsync(IdentityUser? currentUser, bool isEngineer);
        Task<(int totalRecords, List<object> data)> GetTableDataEndTableAsync(IdentityUser? currentUser, bool isEngineer);
    }