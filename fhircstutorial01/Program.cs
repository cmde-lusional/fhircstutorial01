// See https://aka.ms/new-console-template for more information
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Rest;

// connection var for fire server
const string _fhirServer = "http://server.fire.ly";

// read in settings for following fire server connection
var settings = new FhirClientSettings
{
    PreferredFormat = ResourceFormat.Json,
    PreferredReturn = Prefer.ReturnRepresentation
};

// creates a new fire client connecting to a fire server defined in var _fhirServer
var fhirClient = new FhirClient(_fhirServer, settings);


//// GETTING PATIENTS + ENCOUNTER FROM A FIRE SERVER
// getting all bundles for specific patient name 
Bundle patientBundle = fhirClient.Search<Patient>(new string[] {"name=Peter"});


////How to get all encounters + patient names
/*Bundle encounterBundle = fhirClient.Search<Encounter>(null);

Console.WriteLine($"Total: {encounterBundle.Total} Entry count: {encounterBundle.Entry.Count}");

int c = 0;
foreach (Bundle.EntryComponent e in encounterBundle.Entry)
{
   
    Encounter encounter = (Encounter)e.Resource;
    Console.WriteLine($" - EncounterNr {c}: Identifier {encounter.Id, 3}, Patient: {encounter.Subject.Reference, 3}");
    c++;
}*/

Console.WriteLine($"Total: {patientBundle.Total} Entry count: {patientBundle.Entry.Count}");


/* My function is different to the Tutorial!!! Above I'm searching for the patient bundles of a specific name.
   Below I start to initialize counters where pn = the counter for the patients, and encountercounter the counter for 
   the encounters. The date var is later used when displaying the encounters of a patient to show the date.
   
   First we loop until now patient bundle is left. Hereby the fhirClient.Continue(patientBundle) function tells the 
   program to continue looping over all the bundles. Afterwards we loop over all the patient entries inside the bundle.
   The rest is explained in the comments below.
 */
int pn = 0;
int encountercounter = 0;
string date = String.Empty;
// looping over the bundle
while (patientBundle != null)
{
    // list each patient in the bundle
    foreach (Bundle.EntryComponent entry in patientBundle.Entry)
    {
        //Console.WriteLine($" - Entry {pn}: {entry.FullUrl, 3}");

        //Check that the resource is not null
        if (entry.Resource != null)
        {   
            pn++;
            //Get the data for the patient
            Patient patient = (Patient)entry.Resource;
            
            //Get another bundle showing all the encounters for the patient
            Bundle encounterBundle = fhirClient.Search<Encounter>(new string[] { $"subject=Patient/{patient.Id}" });
            
            //Check if there are any encounter for the patient, if not jump out the loop to continue with the next patient.
            if (encounterBundle.Total == 0)
            {
                continue;
            }
            
            Console.WriteLine($" ID: {patient.Id}");
            
            //Check if there is any name inside the patient object
            if (patient.Name.Count > 0)
            {
                Console.WriteLine($" Name: {patient.Name[0].ToString()}");
            }
            
            //loop over all the encounters of the patient
            foreach (Bundle.EntryComponent e in encounterBundle.Entry)
            {
                Encounter encounter = (Encounter)e.Resource;
                
                //check if there is data for the period attribute
                if (encounter.Period != null)
                {
                    date = (encounter.Period.Start).ToString();
                }
                else
                {
                    date = "null";
                }
                
                //write out the data and increase the counter
                Console.WriteLine($" - EncounterID: {encountercounter}, Identifier: {encounter.Id, 3}, Patient: {encounter.Subject.Reference, 3}, Date: {date}");
                encountercounter++;
            }

            encountercounter = 0;
        }
    }
    //The Continue() function takes in a bundle and checks if there is a predecessor and assigns it to the var
    patientBundle = fhirClient.Continue(patientBundle);
}