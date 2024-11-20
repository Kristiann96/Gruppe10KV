# Gruppe10KVprototype

Velkommen til Gruppe10's Kartverket prototype! 
Denne README-filen fungerer som en rask guide for å forstå, sette opp og jobbe med koden i dette repositoriet.

---

## Innholdsfortegnelse

- [Oversikt](#oversikt)
- [Oppsettsinstruksjoner](#oppsettsinstruksjoner)
- [Repository-struktur](#repository-struktur)
  - [Modeller](#modeller)
  - [Grensesnitt](#grensesnitt)
  - [Data Access](#data-access)
- [Teknologier brukt](#teknologier-brukt)
- [Samarbeidspartnere](#samarbeidspartnere)
- [Lisens](#lisens)

---

## Oversikt
Applikasjonen "Gruppe10KVprototype" er basert på et prosjekt fra Kartverket og har som mål å lette brukerrapportering og håndtering av geodata. Den gir et brukervennlig grensesnitt for å sende inn rapporter med geolokasjonsdata og sikrer effektiv håndtering og gjennomgang av disse rapportene for systemadministratorer.

### Hva applikasjonen gjør

- **Brukerrapportering:** Brukere kan sende inn rapporter og feil for geografiske lokasjoner, inkludert geodata (GeoJSON-format).
- **Administratorgjennomgang:** Administratorbrukere ("saksbehandlere") kan effektivt gjennomgå, vurdere og administrere rapporter.
- **Datakompilering:** Systemet gir en detaljert oversikt over forskjellige geodata og brukerinnsendte data.
- **Autentisering og brukeradministrasjon:** Inkluderer et autentiseringssystem for ulike brukertyper, som gjester og registrerte brukere.

---
<br><br>

## Oppsettsinstruksjoner

Klon repositoriet:

    git clone https://github.com/Kristiann96/Gruppe10KV.git


Naviger til prosjektmappen:

    cd Gruppe10KV 

Avhengigheter: Sørg for at alle nødvendige biblioteker og kjøremiljøer er satt opp. Repositoriet er hovedsakelig avhengig av .NET.

Bygg prosjektet: Du kan bygge prosjektet ved hjelp av dine foretrukne .NET-kommandoer eller en IDE som Visual Studio.

Konfigurasjon: Juster konfigurasjonsinnstillingene etter behov. Sørg for at databasetilkoblinger og andre relevante innstillinger er korrekt konfigurert for ditt miljø.

---
<br><br>

## Repository-struktur

Her er en oversikt over de viktigste komponentene i repositoriet:


### Modeller

Modellmappen er strukturert for å inneholde enheter og ulike modellklasser som hovedsakelig brukes til datarepresentasjon.
Nedenfor er en enkel forklaring av en av våre modeller:

  **GeometriModel.cs:**
  
    Inneholder Geometri-klassen som representerer geometridata.
    Egenskaper inkluderer GeometriId, InnmeldingId og GeometriGeoJson.

<br><br>

### Grensesnitt:
Mappen for grensesnitt inneholder alle definisjoner for repositorimønsteret, som legger til rette for dataoperasjoner og abstraksjoner.

  **IGeometriRepository.cs:**
    
    Grensesnitt for geometrirelaterte operasjoner.
    Metoder for å hente alle geometrier, hente geometrier basert på innmeldingId, osv.

  **IInnmeldingRepository.cs:** 

    Grensesnitt for håndtering av "innmeldinger".
    Metoder for henting og oppdatering av "innmeldinger", hente enum-verdier med mer.

  **IVurderingRepository.cs:**
    
    Grensesnitt for håndtering av vurderinger.
    Metoder for å legge til og hente vurderinger.

  **IGjesteinnmelderRepository.cs:**
  
    Grensesnitt for "gjesteinnmeldere".
    Metode for å opprette en "gjesteinnmelder"-oppføring.

  **IInnmelderRepository.cs:**
    
    Grensesnitt for registrerte "innmeldere".
    Metoder for validering og henting av "innmelder"-informasjon.

  **IDataSammenstillingSaksBRepository.cs:**
    
    Grensesnitt for datakompilering.
    Metoder for å hente detaljerte rapporter og paginerte oversikter.

  **ISaksbehandlerRepository.cs:** 
  
    Grensesnitt for "saksbehandlere".
    Metoder for validering og henting av "saksbehandler"-informasjon.

  **ILogginnLogic.cs:**
  
    Grensesnitt for autentiseringsprosesser.
    Håndterer logikk for brukerautentisering.

  **ITransaksjonsRepository.cs:**

    Grensesnitt for håndtering av transaksjonsrelaterte operasjoner.
    Inkluderer metoder for komplett rapportlagring, oppretting og sletting av personer og innmeldere.

<br><br>

### Data Access

DataAccess-mappen inneholder klasser som implementerer grensesnitt for databaseoperasjoner. Dette laget samhandler med databasen ved hjelp av biblioteker som Dapper.

#### Tilkoblinger og repositorieimplementasjoner:

  **DapperDBConnection.cs:** 
  
    Håndterer databasetilkoblinger.
    Bruker MySQL for tilkobling og utføring av kommandoer.

  **DataSammenstillingSaksBRepository.cs:** 
  
    Implementerer komplekse spørringer knyttet til flere datatabeller.
    Henter detaljerte rapporter og paginerte dataoversikter.

  **GjesteinnmelderRepository.cs:**
    
    Håndterer databaseinteraksjoner for "gjesteinnmeldere".
    Tilbyr metoder for oppretting og validering av gjesteoppføringer.

  **GeometriRepository.cs:**
    
    Håndterer databaseinteraksjoner for geometri.
    Henter og oppdaterer geometrier i databasen.

  **InnmeldingRepository.cs:**
    
    Håndterer databaseinteraksjoner for "innmelding".
    Implementerer henting, oppdatering og telling av rapporter. Håndterer enum-feltoperasjoner og oppdateringer av "innmelder".

  **VurderingRepository.cs:**
    
    Håndterer vurderingsrelaterte databaseoppgaver.
    Inneholder metoder for å legge til og vise vurderinger og tilknyttede evalueringer.

  **TransaksjonsRepository.cs:**
    
    Håndterer transaksjonsoperasjoner for oppretting og administrasjon av sammensatte rapportoppføringer.
    Håndterer komplekse transaksjoner mellom enheter.

---
<br><br>

## Teknologier brukt:

- ASP.NET Core MVC: For å bygge webapplikasjonen med et strukturert MVC-mønster.
- C#: Hovedspråk for backend-logikk.
- Dapper ORM: For lette og raske databaseoperasjoner.
- MySQL/MariaDB: Database for lagring og spørring av bruker- og geodata.
- Leaflet.js: JavaScript-bibliotek for interaktive kart.
- Kartverket API: For kartdata og geolokasjonstjenester.

### Hvorfor vi brukte disse teknologiene

- **ASP.NET MVC:** Valgt for sin robuste ramme og innebygde støtte for Model-View-Controller-arkitektur, som gjør det enklere å administrere applikasjonens forskjellige komponenter.
- **Dapper ORM:** Brukt for effektiv databaseinteraksjon med MySQL/MariaDB, gir en lett og fleksibel tilnærming til spørringsutføring.
- **MySQL/MariaDB:** Valgt som databasesystem på grunn av skalerbarhet og støtte.
- **Kartverket API & Leaflet.js:** Integrert for nøyaktig kartlegging og visualisering av geodata, noe som gir brukerne et sanntidsbilde av lokasjoner på et interaktivt kart.

---

## Samarbeidspartnere:
- https://github.com/OlsenLene
- https://github.com/Galescape
- https://github.com/TriggeredBanana
- https://github.com/Kristiann96
- https://github.com/Martinstomnas
- https://github.com/emkaas

---

## Lisens

Prosjektet er lisensiert under MIT-lisensen. <br><br>
Les mer om MIT-lisensen her: https://choosealicense.com/licenses/mit/
