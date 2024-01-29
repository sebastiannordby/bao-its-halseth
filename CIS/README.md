Legg inn user secrets:
```
{
  "ConnectionStrings:DefaultConnection": "Server=DITTPCNAVN;Database=CIS;User Id=sa;Password=secretPassword123;MultipleActiveResultSets=false"
}
```

Lag en database bruker med brukernavn=sa og passord=secretPassword123 eller bytt om ovenfor.

Gjøremål:

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