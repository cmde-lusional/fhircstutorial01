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


//// GETTING PATIENTS FROM A FIRE SERVER
// getting all bundles for specific patient name 
Bundle patientBundle = fhirClient.Search<Patient>(new string[] {"name=chris"});

int pn = 0;

// looping over the bundle
while (patientBundle != null)
{   
    Console.WriteLine($"Total: {patientBundle.Total} Entry count: {patientBundle.Entry.Count}");

    // list each patient in the bundle
    foreach (Bundle.EntryComponent entry in patientBundle.Entry)
    {
        Console.WriteLine($" - Entry {pn,3}: {entry.FullUrl}");

        if (entry.Resource != null)
        {
            Patient patient = (Patient)entry.Resource;
            Console.WriteLine($" ID: {patient.Id}");
            if (patient.Name.Count > 0)
            {
                Console.WriteLine($" Name: {patient.Name[0].ToString()}");
            }
        }

        pn++;
    
    }

    //The Continue() function takes in a bundle and checks if there is a predecessor and assigns it to the var
    patientBundle = fhirClient.Continue(patientBundle);
}