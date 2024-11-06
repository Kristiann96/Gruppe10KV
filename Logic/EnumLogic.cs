using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface;
using LogicInterfaces;

namespace Logic
{
    public class EnumLogic : IEnumLogic
    {
        private readonly IInnmeldingRepository _innmeldingRepository;

        public EnumLogic(IInnmeldingRepository innmeldingRepository)
        {
            _innmeldingRepository = innmeldingRepository;
        }

        private IEnumerable<string> ProcessRawEnumValues(string rawEnumValues)
        {
            if (string.IsNullOrEmpty(rawEnumValues))
                return Enumerable.Empty<string>();

            return rawEnumValues
                .Trim('\'', ')', '(')
                .Split("','")
                .Select(EnumFormatter.ToNormalText)
                .ToList();
        }

        // Dette er den oppdaterte versjonen som erstatter den gamle ValidateEnumValueAsync
        private async Task<bool> ValidateEnumValueAsync(string value, Func<Task<string>> getEnumValuesFunc)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            var rawEnumValues = await getEnumValuesFunc();
            var validValues = rawEnumValues
                .Trim('\'', ')', '(')
                .Split("','");

            var dbFormat = ConvertToDbFormat(value);
            return validValues.Contains(dbFormat) || validValues.Contains(value.ToLower());
        }

        // Nye konverteringsmetoder
        public string ConvertToDbFormat(string displayValue)
        {
            if (string.IsNullOrEmpty(displayValue))
                return string.Empty;

            return displayValue
                .ToLower()
                .Replace(" ", "_");
        }

        public string ConvertToDisplayFormat(string dbValue)
        {
            if (string.IsNullOrEmpty(dbValue))
                return string.Empty;

            return EnumFormatter.ToNormalText(dbValue);
        }

        // Get formatted enum values
        public async Task<IEnumerable<string>> GetFormattedStatusEnumValuesAsync()
        {
            var rawEnumValues = await _innmeldingRepository.GetStatusEnumValuesAsync();
            return ProcessRawEnumValues(rawEnumValues);
        }

        public async Task<IEnumerable<string>> GetFormattedPrioritetEnumValuesAsync()
        {
            var rawEnumValues = await _innmeldingRepository.GetPrioritetEnumValuesAsync();
            return ProcessRawEnumValues(rawEnumValues);
        }

        public async Task<IEnumerable<string>> GetFormattedKartTypeEnumValuesAsync()
        {
            var rawEnumValues = await _innmeldingRepository.GetKartTypeEnumValuesAsync();
            return ProcessRawEnumValues(rawEnumValues);
        }

        public async Task<IEnumerable<string>> GetFormattedInnmelderTypeEnumValuesAsync()
        {
            var rawEnumValues = await _innmeldingRepository.GetInnmelderTypeEnumValuesAsync();
            return ProcessRawEnumValues(rawEnumValues);
        }

        // Validate enum values
        public async Task<bool> ValidateStatusValueAsync(string status)
        {
            return await ValidateEnumValueAsync(status, _innmeldingRepository.GetStatusEnumValuesAsync);
        }

        public async Task<bool> ValidatePrioritetValueAsync(string prioritet)
        {
            return await ValidateEnumValueAsync(prioritet, _innmeldingRepository.GetPrioritetEnumValuesAsync);
        }

        public async Task<bool> ValidateKartTypeValueAsync(string kartType)
        {
            return await ValidateEnumValueAsync(kartType, _innmeldingRepository.GetKartTypeEnumValuesAsync);
        }

        public async Task<bool> ValidateInnmelderTypeValueAsync(string innmelderType)
        {
            return await ValidateEnumValueAsync(innmelderType, _innmeldingRepository.GetInnmelderTypeEnumValuesAsync);
        }
    }
}
