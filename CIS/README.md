Legg inn user secrets:
```
{
  "ConnectionStrings:DefaultConnection": "Server=DITTPCNAVN;Database=CIS;User Id=sa;Password=secretPassword123;MultipleActiveResultSets=false"
}
```

Lag en database bruker med brukernavn=sa og passord=secretPassword123 eller bytt om ovenfor.

Gj�rem�l:

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