# Gruppe10KVprototype

Velkommen til Gruppe10's Kartverket prototype! 
Denne README-filen fungerer som en rask guide for å forstå, sette opp og jobbe med koden i dette repositoriet. For mer detaljer, se vår [Wiki](https://github.com/Kristiann96/Gruppe10KV/wiki/Gruppe10KVprototype-%E2%80%90-Wiki).

---

## Innholdsfortegnelse

- [Oversikt](#oversikt)
- [Eksempler på Bruk](#eksempler-på-bruk)
    - [For innmeldere](#for-innmeldere)
    - [For saksbehandlere](#for-saksbehandlere)
- [Oppsettsinstruksjoner](#oppsettsinstruksjoner)
    - [Forutsetninger](#forutsetninger)
    - [Oppsett](#oppsett)
    - [Docker-konfigurasjon](#docker-konfigurasjon)
    - [Miljøvariabler](#miljøvariabler)
- [Struktur](#struktur)
    - [Systemets arkitektur illustrert](#systemets-arkitektur-illustrert)
- [Teknologier brukt](#teknologier-brukt)
    - [Hvorfor vi brukte disse teknologiene](#hvorfor-vi-brukte-disse-teknologiene)
- [Samarbeidspartnere](#samarbeidspartnere)
- [Lisens](#lisens)

---

## Oversikt
Applikasjonen "Gruppe10KVprototype" er basert på et prosjekt fra Kartverket og har som mål å gjøre brukerrapportering enklere og håndtere ulik geodata. Systemet gir et brukervennlig grensesnitt for å melde inn feil sammen med geolokasjonsdata, og sikrer effektiv håndtering av innmeldinger for saksbehandlere.
Gruppens system introduserer en ny og kreativ løsning der innmeldere kan benytte seg av sporing via GPS, som gjør det enklere å melde inn feil mens man er på tur og oppdager feil i kartet.

### Hva applikasjonen gjør

- **Motta innmeldinger:** Brukere kan melde inn feil på geografiske lokasjoner, inkludert geodata (GeoJSON-format) samt bruke sporing via GPS for å melde inn feil i sanntid.
- **Administratorgjennomgang:** Saksbehandlere kan effektivt gjennomgå, vurdere, administrere og delegere innmeldinger.
- **Datakompilering:** Systemet gir en detaljert oversikt over forskjellige geodata og brukerinnsendte data.
- **Autentisering og brukeradministrasjon:** Inkluderer et autentiseringssystem for ulike brukertyper, som gjester og registrerte brukere.

## Eksempler på bruk
### For innmeldere:
#### 1. Gå til nettsiden.
#### 2. Benytt "Meld inn kartfeil med tegneverktøy" for å melde inn feil i kartet med en gang. Benytt "Bruk GPS for å markere kartfeil" for å ta i bruk sporingstjenesten og melde inn feil ute på tur.
---

<img src="https://github.com/user-attachments/assets/98889096-caa8-460c-9159-b60e92cd48f9" alt="hjemmeside" width="600"/>

---
#### 3. Fyll inn nødvendig informasjon og trykk "Bekreft" for å melde inn feil i kartet.
---

<img src="https://github.com/user-attachments/assets/35cf98e4-6c81-429f-b8e8-2ab304baf5f9" alt="innmelding1" width="600"/>
<img src="https://github.com/user-attachments/assets/d0b8a568-eafb-445f-a890-c178630bdeeb" alt="innmelding2" width="600"/>

---
#### 4. Opprett bruker eller logg inn dersom du vil motta oppdateringer på din sak. Legg igjen din e-post som gjesteinnmelder om du ikke vil opprette bruker. Oppretter man ikke bruker vil saken knyttes til innmelders e-post.
---

<img src="https://github.com/user-attachments/assets/81a3ca90-3556-4c19-b021-ff50e4724826" alt="opprettBruker" width="600"/>

---
#### 5. Oppretter man bruker kan man følge status på innmelding under "Mine Innmeldinger".
---

<img src="https://github.com/user-attachments/assets/01953ab8-ba13-4c30-90a0-d4b95ee1022f" alt="mineInnmeldinger" width="600"/>

---
#### 6. Man kan bidra til kartforbedring ved å bekrefte eller avkrefte en anonym brukers innmelding og legge ved kommentar.
---
<img src="https://github.com/user-attachments/assets/7356fe07-ede5-4053-9f3b-56fbe635ac54" alt="forbedreKart1" width="600"/>
<img src="https://github.com/user-attachments/assets/a47a56a8-a328-4950-a1df-7a2b77127668" alt="forbedreKart2" width="600"/>

---
<br></br>

### For saksbehandlere:
#### 1. Logg inn med saksbehandlerkonto.
---

<img src="https://github.com/user-attachments/assets/298cbd29-ae28-4fc0-9485-76dbc14d8a5e" alt="saksbehandlerInnlogging" width="600"/>

---
#### 2. Gå til "Innmeldingsoversikt" for å se en liste over alle innmeldinger. Gå til "Kart over innmeldinger" for å se alle innmeldinger på kartet. På "Kart over innmeldinger" kan man også markere et område for å få alle innmeldte saker i markert område som vist nedenfor.
---

#### Innmeldingsoversikt:

<img src="https://github.com/user-attachments/assets/0aa5467f-f57d-4994-b355-2c775d2480d8" alt="innmeldingOversikt" width="600"/>

---
#### Kart over innmeldinger:

<img src="https://github.com/user-attachments/assets/e129e070-ad2f-466f-8e13-522adc698b22" alt="innmeldingKart" width="600"/>

---
#### Markerte innmeldinger på kart:

<img src="https://github.com/user-attachments/assets/c30c4b6b-8810-4374-ba37-ecc0889a695d" alt="markerteInnmeldinger" width="600"/>

---
#### 3. Velg en innmelding for å gi vurdering eller oppdatere status.
---

<img src="https://github.com/user-attachments/assets/b6d08f1f-c602-4ce4-b6fa-17e452bb215b" alt="enkeltInnmeldingSaksB" width="600"/>

---
<br><br>

## Oppsettsinstruksjoner

### Forutsetninger:

- .NET 8.0 SDK
- Docker Desktop
- Visual Studio 2022 eller JetBrains Rider
- Mermaid extension (valgfritt - for å se database/dataflyt-diagrammer)
- SSH-nøkkel for databasetilgang

### Oppsett
#### Prosjektet kan utvikles i:

- Visual Studio 2022
- JetBrains Rider

#### Installasjonstrinn:
- Klon repositoriet
    
        git clone https://github.com/Kristiann96/Gruppe10KV.git
        
- Konfigurer connection strings i appsettings.json
- Kjør EF migrasjoner for identity database
- Start Docker container
- Bygg og kjør applikasjonen

### Docker-konfigurasjon
#### Prosjektet bruker Docker Compose for enkel oppstart:

    services:
      gruppe10kvprototype:
        image: gruppe10kvprototype
        build:
          context: .
          dockerfile: Gruppe10KVprototype/Dockerfile

### Miljøvariabler
#### Følgende variabler må konfigureres i appsettings.json:

- MariaDbConnection_kartverket: Connection string til hoveddatabase
- MariaDbConnection_login_server: Connection string til identity database


---
<br><br>

## Struktur

Gruppens system benytter seg av en repository struktur for enklere vedlikehold av kode. Ved å benytte repository struktur separeres forretningslogikken fra datatilganglogikken og hjelper med å oppnå løs kobling i systemet.
Under er en oversikt over de viktigste komponentene i gruppens system og en kort forklaring av hver komponent:

    ├── Gruppe10KVprototype
    ├── AuthDataAccess
    ├── AuthInterface
    ├── DataAccess
    ├── Interfaces
    ├── Logic
    ├── LogicInterfaces
    ├── Models
    ├── Services
    ├── ServicesInterfaces
    ├── ViewModels

| **Komponent**         | **Beskrivelse**                                                                                     |
|------------------------|-----------------------------------------------------------------------------------------------------|
| **Gruppe10KVprototype** | Hovedprosjektet som inneholder applikasjonen.                                                      |
| **AuthDataAccess**      | Inneholder implementasjon for håndtering av autentisering og autorisering av brukere.              |
| **AuthInterface**       | Definerer grensesnittet for funksjonalitet relatert til autentisering.                              |
| **DataAccess**          | Ansvarlig for datatilgang og henting av data fra database ved bruk av SQL.                          |
| **Interfaces**          | Definerer ulike repositorier og tjenestegrensesnitt som muliggjør abstraksjon og løs kobling.         |
| **Logic**               | Inneholder forretningslogikk som f.eks "InnmeldingLogic" som validerer innmeldinger.                                          |
| **LogicInterfaces**     | Inneholder grensesnittene for applikasjonens logikkomponenter.                                      |
| **Models**              | Lagrer datamodellene og entitetene som brukes på tvers av applikasjonen.                           |
| **Services**            | Implementerer forretningslogikken og koordinerer samspillet mellom repositorier og andre komponenter. |
| **ServicesInterfaces**  | Definerer grensesnittene for "Services".                                           |
| **ViewModels**          | Inneholder view-modeller som brukes for å presentere data i brukergrensesnittet.                   |


### Systemets arkitektur illustrert:
---

<img src="https://github.com/user-attachments/assets/942c76c8-428e-4a02-aedd-b03daae84d45" alt="Eksempelbilde 11" width="600"/>

---
<br><br>


## Teknologier brukt:

- ASP.NET Core MVC: For å bygge webapplikasjonen med et strukturert MVC-mønster.
- C#: Hovedspråk for backend-logikk.
- Dapper ORM: For lette og raske databaseoperasjoner.
- MySQL/MariaDB: Database for lagring og spørring av bruker- og geodata.
- Leaflet.js: JavaScript-bibliotek for interaktive kart.
- Kartverket API: For kartdata og geolokasjonstjenester.
- Entity Framework & Identity: For autorisasjon og autentisering.

### Hvorfor vi brukte disse teknologiene

- **ASP.NET MVC:** Valgt for sin robuste ramme og innebygde støtte for Model-View-Controller-arkitektur, som gjør det enklere å administrere applikasjonens forskjellige komponenter.
- **Dapper ORM:** Brukt for effektiv databaseinteraksjon med MySQL/MariaDB, gir en lett og fleksibel tilnærming til spørringsutføring.
- **MySQL/MariaDB:** Valgt som databasesystem på grunn av skalerbarhet og støtte.
- **Kartverket API & Leaflet.js:** Integrert for nøyaktig kartlegging og visualisering av geodata, noe som gir brukerne et sanntidsbilde av lokasjoner på et interaktivt kart.
- **Entity Framework ORM & Identity:** Brukt for autorisasjon og autentisering grunnet automatisk integrasjon mellom identitet og Entity Framework.

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
