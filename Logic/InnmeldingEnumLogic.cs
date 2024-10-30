using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface;

namespace Logic
{
    public class InnmeldingEnumLogic : IInnmeldingEnumLogic
    {
        private readonly IInnmeldingRepository _innmeldingRepository;

        public InnmeldingEnumLogic(IInnmeldingRepository innmeldingRepository)
        {
            _innmeldingRepository = innmeldingRepository;
        }

        public async Task<IEnumerable<string>> GetFormattedStatusEnumValuesAsync()
        {
            var rawEnumValues = await _innmeldingRepository.GetStatusEnumValuesAsync();

            return rawEnumValues
                .Trim('\'')
                .Split("','")
                .Select(EnumFormatter.ToNormalText)
                .ToList();
        }
    }

    //det nedefor er en hjelpeklasse som formaterer enum-verdier til normal tekst - burde kanskje vært i en egen fil
    public static class EnumFormatter
    {
        public static string ToNormalText(string value)
        {
            return string.Join(" ", value.Split('_'))
                .ToLower()
                .Trim()
                .ReplaceFirstCharToUpper();
        }
    }
    //dette burde vært i en egen fil det også
    public static class StringExtensions
    {
        public static string ReplaceFirstCharToUpper(this string input)
        {
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }


}
