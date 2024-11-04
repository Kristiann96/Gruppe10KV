namespace LogicInterfaces
{
    public interface IInnmeldingEnumLogic
    {
        Task<IEnumerable<string>> GetFormattedStatusEnumValuesAsync();
    }
}
