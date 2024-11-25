namespace LogicInterfaces
{
    public interface IEnumLogic
    {
        Task<IEnumerable<string>> GetFormattedStatusEnumValuesAsync();
        Task<IEnumerable<string>> GetFormattedPrioritetEnumValuesAsync();
        Task<IEnumerable<string>> GetFormattedKartTypeEnumValuesAsync();
        Task<IEnumerable<string>> GetFormattedInnmelderTypeEnumValuesAsync();
        Task<bool> ValidateStatusValueAsync(string status);
        Task<bool> ValidatePrioritetValueAsync(string prioritet);
        Task<bool> ValidateKartTypeValueAsync(string kartType);
        Task<bool> ValidateInnmelderTypeValueAsync(string innmelderType);
        string ConvertToDbFormat(string displayValue);
        string ConvertToDisplayFormat(string dbValue);
    }
}
