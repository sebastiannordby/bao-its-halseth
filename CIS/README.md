
For å kjøre applikasjonen må følgende inn:

	Legg inn user secrets:
	```
	{
	  "ConnectionStrings:DefaultConnection": "Server=DITTPCNAVN;Database=CIS;User Id=sa;Password=secretPassword123;MultipleActiveResultSets=True;TrustServerCertificate=True;Connection Timeout=120"
	}
	```

	Lag en database bruker med brukernavn=sa og passord=secretPassword123 eller bytt om ovenfor.


For å kjøre tailwind må man ta CD in i "CIS" mappen og kjøre "npm run tail"

Gjøremål Andreas Halseth:

Import av kunde
- Lag en excel fil med kundedata fra 
- Les kunder fra excel fil
- Bruk ICustomerService for å importere kunder
- Import definisjonen er klassen: CustomerImportDefinition
- Du må lage en spørring som tar med nåværende kundenummer også alle felter fra kunde
	- Feltene som skal til butikk må leses inn i Store-objektet
	
Visning av kunde
- Sett opp en grid imot modellen CustomerView
- Som en test etter importen kan du bruke: ICustomerViewRepository.List

Gjøremål Martin Sørensen:

For å bli litt kjent med Blazor har jeg laget en liten oppgave til deg.

Jeg vil at applikasjonen skal se nogenlunde slik ut:
https://media.discordapp.net/attachments/1192916915618840729/1201457244655583262/image.png?ex=65c9e341&is=65b76e41&hm=c10f38b936a8c53e2e1edb8820b53eff089c08235df5c286c5a22cc3b965fc6b&=&format=webp&quality=lossless

Jeg vil også at landingssiden/index/home skal være et dashboard:
https://images.klipfolio.com/website/public/4d789bf2-a6d2-45ea-87e7-38e131f9d354/sales%20dashboard.png

## Hvordan importere eksisterende data?
Du må ha lagt inn connectionstring til en lokal mssqlserver som vist øverst.
Du trenger ikke å opprette en database siden dette skjer automatisk.


Under Onedriven "BAO - Its Halseth" finnes det en mappe som heter utvikling,
under der ligger følgende filer:
- "Vare_Import_Definisjon.xlsx"
- "Kunde_Import_Definisjon.xlsx"

Vare-fil importeres i "Varer"-bildet også videre.

Trykk på fanen import og følg stegene der.

Må importeres i følgende rekkefølge:
- Varer
- Kunder
- Ordre - Ikke ferdig enda
