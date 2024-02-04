
For � kj�re applikasjonen m� f�lgende inn:

	Legg inn user secrets:
	```
	{
	  "ConnectionStrings:DefaultConnection": "Server=DITTPCNAVN;Database=CIS;User Id=sa;Password=secretPassword123;MultipleActiveResultSets=True;TrustServerCertificate=True;Connection Timeout=120"
	}
	```

	Lag en database bruker med brukernavn=sa og passord=secretPassword123 eller bytt om ovenfor.


For � kj�re tailwind m� man ta CD in i "CIS" mappen og kj�re "npm run tail"

Gj�rem�l Andreas Halseth:

Import av kunde
- Lag en excel fil med kundedata fra 
- Les kunder fra excel fil
- Bruk ICustomerService for � importere kunder
- Import definisjonen er klassen: CustomerImportDefinition
- Du m� lage en sp�rring som tar med n�v�rende kundenummer ogs� alle felter fra kunde
	- Feltene som skal til butikk m� leses inn i Store-objektet
	
Visning av kunde
- Sett opp en grid imot modellen CustomerView
- Som en test etter importen kan du bruke: ICustomerViewRepository.List

Gj�rem�l Martin S�rensen:

For � bli litt kjent med Blazor har jeg laget en liten oppgave til deg.

Jeg vil at applikasjonen skal se nogenlunde slik ut:
https://media.discordapp.net/attachments/1192916915618840729/1201457244655583262/image.png?ex=65c9e341&is=65b76e41&hm=c10f38b936a8c53e2e1edb8820b53eff089c08235df5c286c5a22cc3b965fc6b&=&format=webp&quality=lossless

Jeg vil ogs� at landingssiden/index/home skal v�re et dashboard:
https://images.klipfolio.com/website/public/4d789bf2-a6d2-45ea-87e7-38e131f9d354/sales%20dashboard.png

## Hvordan importere eksisterende data?
Du m� ha lagt inn connectionstring til en lokal mssqlserver som vist �verst.
Du trenger ikke � opprette en database siden dette skjer automatisk.


Under Onedriven "BAO - Its Halseth" finnes det en mappe som heter utvikling,
under der ligger f�lgende filer:
- "Vare_Import_Definisjon.xlsx"
- "Kunde_Import_Definisjon.xlsx"

Vare-fil importeres i "Varer"-bildet ogs� videre.

Trykk p� fanen import og f�lg stegene der.

M� importeres i f�lgende rekkef�lge:
- Varer
- Kunder
- Ordre - Ikke ferdig enda
