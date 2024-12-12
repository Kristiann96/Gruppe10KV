using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface;
using Interfaces;
using LogicInterfaces;
using ServicesInterfaces;
using ViewModels;

namespace Services
{
    public class KartvisningEnEllerFlereInnmeldingSaksBService : IKartvisningEnEllerFlereInnmeldingSaksBService
    {
        private readonly IInnmeldingRepository _innmeldingRepo;
        private readonly IInnmelderRepository _innmelderRepo;
        private readonly ISaksbehandlerRepository _saksbehandlerRepo;
        private readonly IGeometriRepository _geometriRepo;
        private readonly IVurderingRepository _vurderingRepo;
        private readonly IEnumLogic _enumLogic;

        public KartvisningEnEllerFlereInnmeldingSaksBService(
            IInnmeldingRepository innmeldingRepo,
            IInnmelderRepository innmelderRepo,
            ISaksbehandlerRepository saksbehandlerRepo,
            IGeometriRepository geometriRepo,
            IVurderingRepository vurderingRepo,
            IEnumLogic enumLogic)
        {
            _innmeldingRepo = innmeldingRepo;
            _innmelderRepo = innmelderRepo;
            _saksbehandlerRepo = saksbehandlerRepo;
            _geometriRepo = geometriRepo;
            _vurderingRepo = vurderingRepo;
            _enumLogic = enumLogic;
        }

        public async Task<KartvisningEnEllerFlereInnmeldingSaksBViewModel> HentKartvisningForEnkeltInnmeldingAsync(
            int innmeldingId)
        {
            var viewModel = new KartvisningEnEllerFlereInnmeldingSaksBViewModel();
            viewModel.AlleInnmeldinger.Add(await KartvisningsOversiktViewModelAsync(innmeldingId));
            return viewModel;
        }

        public async Task<KartvisningEnEllerFlereInnmeldingSaksBViewModel> HentKartvisningForFlereInnmeldingerAsync(
            IEnumerable<int> innmeldingId)
        {
            var viewModel = new KartvisningEnEllerFlereInnmeldingSaksBViewModel();
            foreach (var id in innmeldingId)
            {
                viewModel.AlleInnmeldinger.Add(await KartvisningsOversiktViewModelAsync(id));
            }

            return viewModel;
        }

        private async Task<InnmeldingOversiktViewModel> KartvisningsOversiktViewModelAsync(int innmeldingId)
        {
            var innmelding = await _innmeldingRepo.HentInnmeldingOppsummeringAsync(innmeldingId);
            var innmelder = await _innmelderRepo.HentInnmelderTypeAsync(innmeldingId);
            var (saksbehandler, person) = await _saksbehandlerRepo.HentSaksbehandlerNavnAsync(innmeldingId);
            var geometri = await _geometriRepo.GetGeometriByInnmeldingIdAsync(innmeldingId); ;
            var (bekreftelser, avkreftelser) = await _vurderingRepo.HentAntallVurderingerAsync(innmeldingId);
            var kommentarer = await _vurderingRepo.HentKommentarerForInnmeldingAsync(innmeldingId);

            return new InnmeldingOversiktViewModel
            {
                InnmeldingId = innmelding.InnmeldingId,
                Tittel = innmelding.Tittel,
                Status = _enumLogic.ConvertToDisplayFormat(innmelding.Status),
                Prioritet = _enumLogic.ConvertToDisplayFormat(innmelding.Prioritet),
                KartType = _enumLogic.ConvertToDisplayFormat(innmelding.KartType),
                InnmelderType = innmelder != null
                    ? _enumLogic.ConvertToDisplayFormat(innmelder.InnmelderType)
                    : string.Empty,
                SaksbehandlerFornavn = person?.Fornavn,
                SaksbehandlerEtternavn = person?.Etternavn,
                Geometri = new GeometriInfo
                {
                    GeoJson = geometri.GeometriGeoJson
                },
                AntallBekreftelser = bekreftelser,
                AntallAvkreftelser = avkreftelser,
                Kommentarer = kommentarer.ToList()
            };
        }
    }

}
