using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Serialization;

namespace Assignment1
{
    /// <summary>
    /// Maps dictionary entries containing disaster related data to turtle rdf
    /// </summary>
    class DisasterMapper : ITurtleMapper
    {
        /// <summary>
        /// Delegate that performes the desired operation on the given value
        /// </summary>
        /// <param name="sb">StringBuilder</param>
        /// <param name="value">value</param>
        public delegate void MappingHandler(StringBuilder sb, string value);

        static string serviceUrl = "http://lookup.dbpedia.org/api/search.asmx/KeywordSearch?QueryString=";
        static Dictionary<string, MappingHandler> keyFunctionMap = new Dictionary<string, MappingHandler>();
        static Dictionary<string, string> valueMap = new Dictionary<string, string>();

        #region IMapper

        public void Init()
        {
            InitValueMap();
            InitKeyFunctionMap();
        }

        public void AddVocabularyDeclaration(StringBuilder sb)
        {
            sb.AppendLine(
@"@prefix sws: <http://vocab.inf.ed.ac.uk/sws/a1546360/sws> .
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix org: <http://www.w3.org/ns/org#> .
@prefix skos: <http://www.w3.org/2004/02/skos/core#> .
@prefix db: <http://dbpedia.org/resource/classes#> .
@prefix foaf: <http://xmlns.com/foaf/0.1/> .
");
        }

        public void BeginNewEntry(StringBuilder sb, int line)
        {
            sb.AppendFormat("\r\nsws:{0} a db:organisation;\r\n", line);
        }

        public void MapEntry(string key, string value, StringBuilder sb)
        {
            keyFunctionMap[key](sb, value);
        }

        #endregion

        /// <summary>
        /// Fills dictionary that maps csv column entries to turtle classes
        /// </summary>
        private void InitValueMap()
        {
            valueMap.Add("NGO - Local", "sws:NgoLocal");
            valueMap.Add("NGO - International", "sws:NgoInternational");
            valueMap.Add("Corporation", "sws.Corporation");
            valueMap.Add("Government", "sws.Government");
            valueMap.Add("Development Bank", "sws:DevelopmentBank");
            valueMap.Add("Political Party", "sws:PoliticalParty");
            valueMap.Add("Individuals", "sws:Individuals");
            valueMap.Add("UN Agency", "sws:UNAgency");
            valueMap.Add("Early Recovery", "sws:EarlyRecovery");
            valueMap.Add("Shelter & NFI", "sws:ShelterAndNFI");
            valueMap.Add("Health", "sws:Health");
            valueMap.Add("WASH", "sws:Wash");
            valueMap.Add("Assessments", "sws:Assessments");
            valueMap.Add("Nutrition", "sws:Nutrition");
            valueMap.Add("Funding", "sws:Funding");
            valueMap.Add("Protection", "sws:Protection");
            valueMap.Add("Education", "sws:Education");
            valueMap.Add("Search & Rescue", "sws:SearchAndRescue");
            valueMap.Add("Food Security", "sws:FoodSecurity");
            valueMap.Add("Logistics", "sws:Logistics");
            valueMap.Add("Animal Rescue", "sws:AnimalRescue");
            valueMap.Add("Telecom", "sws:Telecom");
            valueMap.Add("Camp Coordination", "sws:CampCoordination");
        }


        /// <summary>
        /// Fills dictionary that maps csv columns to a handler function
        /// </summary>
       private void InitKeyFunctionMap()
        {
            keyFunctionMap.Add("Parent Organization", ParentOrganizationHandler);
            keyFunctionMap.Add("Subsidiary Organization", SubsidiaryOrganization);
            keyFunctionMap.Add("Acronym", Acronym);
            keyFunctionMap.Add("Org Type", OrgType);
            keyFunctionMap.Add("Country of origin", Country);
            keyFunctionMap.Add("Cluster 1", Cluster);
            keyFunctionMap.Add("Cluster 2", Cluster);
            keyFunctionMap.Add("Cluster 3", Cluster);
            keyFunctionMap.Add("Cluster 4", Cluster);
            keyFunctionMap.Add("Cluster 5", Cluster);
            keyFunctionMap.Add("Name/Type of response", Response);
            keyFunctionMap.Add("Capacity", Capacity);
            keyFunctionMap.Add("Organization Website", OrganizationWebsite);
            keyFunctionMap.Add("Organization Facebook", OrganizationFacebook);
            keyFunctionMap.Add("Organization Twitter", OrganizationTwitter);
            keyFunctionMap.Add("Donation Links", DonationLinks);
            keyFunctionMap.Add("News and Source Links", NewsLinks);
        }

       /// <summary>
        /// Queries service for given resource
        /// </summary>
        /// <param name="query">e.g. "USA"</param>
        /// <returns>service response</returns>
        private static string QueryLookupService(string query)
        {
            string responseFromServer = null;

            WebRequest request = WebRequest.Create(String.Concat(serviceUrl, query));
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

        #region Handlers
        private static void ParentOrganizationHandler(StringBuilder sb, string value)
        {
            if (!value.StartsWith("null"))
                sb.AppendFormat("   skos:prefLabel \"{0}\"^^xsd:string ;\r\n", value);
        }

        private static void SubsidiaryOrganization(StringBuilder sb, string value)
        {
            if (!value.StartsWith("null"))
                sb.AppendFormat("   org:hasSubOrganization \"{0}\"^^xsd:string ;\r\n", value);
        }

        private static void Acronym(StringBuilder sb, string value)
        {
            if (!value.StartsWith("null"))
                sb.AppendFormat("   skos:altLabel \"{0}\"^^xsd:string ;\r\n", value);
        }

        private static void OrgType(StringBuilder sb, string value)
        {
            if (!value.StartsWith("null") && valueMap.ContainsKey(value))
                sb.AppendFormat("   sws:orgType {0} ;\r\n", valueMap[value]);
        }

        private static void Country(StringBuilder sb, string value)
        {
            if (!value.StartsWith("null"))
            {
                if (!value.ToLower().StartsWith("international"))
                {

                    string response = QueryLookupService(value);

                    XmlSerializer serializer = new XmlSerializer(typeof(ArrayOfResult));

                    using (TextReader reader = new StringReader(response))
                    {
                        ArrayOfResult res = (ArrayOfResult)serializer.Deserialize(reader);

                        if (res != null && res.Items.Count > 0)
                            sb.AppendFormat("   db:country <{0}> ;\r\n", res.Items[0].URI);
                        else
                            sb.AppendFormat("   db:country \"{0}\"^^xsd:string ;\r\n", value);
                    }
                }
                else
                {
                    sb.AppendFormat("   db:country \"{0}\"^^xsd:string ;\r\n", value);
                }
            }
        }

        private static void Cluster(StringBuilder sb, string value)
        {
            if (!value.StartsWith("null") && valueMap.ContainsKey(value))
                sb.AppendFormat("   org:classification {0} ;\r\n", valueMap[value]);
        }

        private static void Response(StringBuilder sb, string value)
        {
            if (!value.StartsWith("null"))
                sb.AppendFormat("   sws:response \"{0}\"@en ;\r\n", value);
        }

        private static void Capacity(StringBuilder sb, string value)
        {
            if (!value.StartsWith("null"))
                sb.AppendFormat("   sws:capacity \"{0}\"@en ;\r\n", value);
        }

        private static void OrganizationWebsite(StringBuilder sb, string value)
        {
            if (!value.StartsWith("null") && !value.StartsWith("NA"))
                sb.AppendFormat("   foaf:homepage <{0}> ;\r\n", value);
        }

        private static void OrganizationFacebook(StringBuilder sb, string value)
        {
            if (value.Contains("facebook.com/"))
            {
                sb.AppendFormat(@"   foaf:account [
      a foaf:OnlineAccount ;
      foaf:accountServiceHomepage <http://www.facebook.com/> ;
      foaf:accountName ""{0}""^^xsd:string
   ] ;
", value.Split(new string[] { "facebook.com/" }, StringSplitOptions.RemoveEmptyEntries)[1]);
            }
        }

        private static void OrganizationTwitter(StringBuilder sb, string value)
        {
            if (value.Contains("twitter.com/"))
            {
                sb.AppendFormat(@"   foaf:account [
      a foaf:OnlineAccount ;
      foaf:accountServiceHomepage <http://www.twitter.com/> ;
      foaf:accountName ""{0}""^^xsd:string
   ] ;
", value.Split(new string[] { "twitter.com/" }, StringSplitOptions.RemoveEmptyEntries)[1]);
            }
        }

        private static void DonationLinks(StringBuilder sb, string value)
        {
            if (!value.StartsWith("null") && !value.StartsWith("NA"))
            {
                foreach (string link in value.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    sb.AppendFormat("   sws:donationLink <{0}> ;\r\n", link);
                }
            }
        }

        private static void NewsLinks(StringBuilder sb, string value)
        {
            if (!value.StartsWith("null") && !value.StartsWith("NA"))
            {
                foreach (string link in value.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    sb.AppendFormat("   sws:newsAndSourceLink <{0}> ;\r\n", link);
                }
            }
        }

        #endregion
        
    }
}
