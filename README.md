# Lune

Digitalt atferdsskjema for demensomsorg.

Lune er en digital versjon av papirskjemaet som brukes på sykehjem for å registrere atferd gjennom døgnet. Pleiere registrerer hva som skjedde og når, og appen viser mønstre med fargekoder per time.

> 🚧 Under utvikling

## Tech stack

- **Backend:** ASP.NET Core (.NET 10), Entity Framework Core
- **Database:** PostgreSQL
- **Frontend:** React + TypeScript (kommer)
- **Kjøring:** Docker Compose

## Kom i gang

```bash
git clone https://github.com/<brukernavn>/lune.git
cd lune
docker compose up -d
cd backend
dotnet run --project Lune.Api
```

## Personvern

Dette er et personlig hobbyprosjekt. All data i prosjektet er oppdiktet, og prosjektet har ingen tilknytning til noen arbeidsplass.
