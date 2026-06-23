# Register

## **Beskrivelse av funksjonalitet:**

Registeret er et verktøy for å administrere registre/kodelister.

**Teknisk:**

For de fleste registrene ligger referansen til register-oppføringen i tabellen Registers, og selve oppføringene/innholdet i registeret i RegisterItems-tabellen.

**Applikasjoner under registeret:**

**Register - hovedmodul:**

Dokumentregistre:

* Produktark
* Produktspesifikasjoner
* Tegneregler
* GML-applkasjonsskjema
* Nasjonale standarder og veiledere
* Standarder
* Styrende dokumenter
  

Kodelister:

* Kodelister
* Metadatakodelister
* Organisasjonsregister
* EPSG-koder
* SOSI-kodelister
  

Statusregistre:

* FAIR-register
* Mareano statusregister
* DOK statusregister
* Inspire statusregister
* Geodatalov statusregister
* DOK kommunalt
* DOK - dekningskart
  

Andre registre:

* Navnerom
* Varsler (eget API)
* Mottaksløsninger ligger i registeret men i egen tabell
  

**Digital kartografi (**[**https://register.geonorge.no/kartografi**](https://register.geonorge.no/kartografi "https://register.geonorge.no/kartografi")**)**

**Symbolregister (**[**https://register.geonorge.no/symbol**](https://register.geonorge.no/symbol "https://register.geonorge.no/symbol")**)**

**Geolett-register (**[**https://register.geonorge.no/geolett**](https://register.geonorge.no/geolett "https://register.geonorge.no/geolett")**)**

**Objektregister (**[**https://objektkatalog.geonorge.no/**](https://objektkatalog.geonorge.no/ "https://objektkatalog.geonorge.no/")**)**

**URL**:

* **Dev:** [http://register.dev.geonorge.no/](http://register.dev.geonorge.no/ "http://register.dev.geonorge.no/")
* **Test:** [https://register.test.geonorge.no/](https://register.test.geonorge.no/ "https://register.test.geonorge.no/")
* **Prod:** [https://register.geonorge.no/](https://register.geonorge.no/ "https://register.geonorge.no/")
  

**Kildekode:** [https://github.com/kartverket/Geonorge.Register](https://github.com/kartverket/Geonorge.Register "https://github.com/kartverket/Geonorge.Register") 

**Oversikt kildekode**:

Applikasjonen er utviklet med C# og .NET framework.

* **/Kartverket.Register** Web applikasjon: ASP.NET MVC 5
* **/Kartverket.Register.Tests** Tester av applikasjonen
  

NB! Trenger å installere [https://www.ghostscript.com/](https://www.ghostscript.com/ "https://www.ghostscript.com/") for å generere thumbnails av pdf.

Ulike registertyper

**Datasett**

* Egentlig duplikatregister siden datasett opprettes i editoren og vises i kartkatalogen
  

**Det offentlige kartgrunnlaget**

* Eget register over hvor hvilke datasett som inngår i det offentlige 
* Går litt i beina på datapakkene som er innført i kartkatalogen. 
  

**Produktark**

* Håndteres som et dokument
  

**Produktspesifikasjoner**

* Håndteres som et dokument
  

**Tegnerregler**

* Håndteres som et dokument
  

**Veiledningsdokumenter**

* Håndteres som et dokument
  

**Dokumenter har godkjenningsprosess og versjonering.**

Alle objekter har innsender, men ikke alle har eier. 

Alle objekter i databasen er et RegisterItem.

**Roller**

Brukere har forskjellige roller og det styrer blant annet hvilke registre de har tilgang til å legge til/redigere og slette data i og hvilke data...  
OBS! En bruker kan ha flere roller...

* **nd.metadata\_admin** er admin og har alle rettigheter.
  * Kan blant annet styre rettigheter på om et register skal kunne redigeres av kun admin eller admin og editor. Det gjøres ved å endre tilgangen til et valgt register.
    * Eksempel:  
      Det offentlige kartgrunnlaget: Kun admin kan legge til, redigere og slette.  
      Produktark: Admin og Editor kan endre.
  * Redigere alle register
  * Legge til, endre og slette alle registeroppføringer
* **nd.metadata\_editor** er editor og får låv til å legge til i registre som har tilgang for editorer.
  * Endre og slette egne oppføringer - oppføringer hvor  brukeren står som eier
  * **Kommunebruker**  
    Dersom brukeren er tilknyttet en kommune har den også lov til å legge inn kommunale datasett for den aktuelle kommunen. Gjelder registeret: "Det offentlige kartgrunnlaget - kommunalt".   
    Dette registeret er ikke åpent for andre editorbrukere.  
* **dok\_admin** kan redigere for alle kommuner i "Det offentlige kartgrunnlaget - kommunalt"
  

**API**

Api er dokumentert her:  [https://register.geonorge.no/help](https://register.geonorge.no/help "https://register.geonorge.no/help")

Eksempelkall:

[http://register.geonorge.no/api/produktspesifikasjoner](http://register.geonorge.no/api/produktspesifikasjoner "http://register.geonorge.no/api/produktspesifikasjoner") returnerer alle produktspesifikasjoner som er registert i Geonorge.

[http://register.geonorge.no/api/epsg-koder](http://register.geonorge.no/api/epsg-koder "http://register.geonorge.no/api/epsg-koder") returnerer alle EPSG-koder som ligger i registeret i Geonorge.

Register benytter solr indeks for søk: [https://github.com/kartverket/Geonorge.Kartkatalog.Solr/tree/master/solr/register](https://github.com/kartverket/Geonorge.Kartkatalog.Solr/tree/master/solr/register "https://github.com/kartverket/Geonorge.Kartkatalog.Solr/tree/master/solr/register")