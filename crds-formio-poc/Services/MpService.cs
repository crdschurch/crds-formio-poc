using System.Collections.Generic;
using System.Linq;
using Crossroads.Web.Common.Extensions;
using Crossroads.Web.Common.MinistryPlatform;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace crds_formio_poc.Services
{
    public class MpService
    {
        private MinistryPlatformRestRepository mpRest;
        private string apiToken = "putvalidtokenhere";
        private RestClient ministryPlatformRestClient = new RestClient("https://adminint.crossroads.net/ministryplatformapi");
        private RestClient authenticationRestClient = new RestClient("https://adminint.crossroads.net/ministryplatform/oauth");

        public MpService()
        {
            mpRest = new MinistryPlatformRestRepository(ministryPlatformRestClient, authenticationRestClient);
        }

        public string GetFormPreFillData(string formData)
        {
            var submission = "{data: ";  //"{data:{profileModuleFirstName:\"Andy\", profileModuleLastName:\"Canterbury\"}}";

            JObject form = JObject.Parse(formData);
            //All components with data table property? 
            var components = form.SelectTokens("..components[?(@.properties.data_table)]");
            
            //build a list
            var data = new List<FieldInfo>();
            foreach (var component in components)
            {
                data.Add( new FieldInfo{FieldName = component.SelectToken("..key").ToString(), DataTable = component.SelectToken("..data_table").ToString(), DataField = component.SelectToken("..data_field").ToString()});
            }

            //build query
            foreach (var table in data.Select(d => d.DataTable).Distinct())
            {
                //get columns for this table
                var cols = data.Where(d => d.DataTable == table);
                var columnList = cols.Aggregate("", (current, col) => current + (col.DataField + " AS " + col.FieldName + ","));
                columnList = columnList.TrimEnd(',');
                var recordId = 3717387;
                submission += GetMPData(table, recordId, columnList).ToString();
            }

            submission += "}";
            return submission;
        }

        private JObject GetMPData(string tableName, int recordId, string columnNames)
        {
            var request = new RestRequest($"/tables/{tableName}/{recordId}", Method.GET);
            request.AddHeader("Authorization", $"Bearer {apiToken}");
            request.AddQueryParameterIfSpecified("$select", columnNames);
            var response = ministryPlatformRestClient.Execute(request);
            response.CheckForErrors("Error getting Data", true);
            var junk = JArray.Parse(response.Content);
            var data = junk.Children<JObject>().FirstOrDefault();
            return data;
        }
    }

    class FieldInfo
    {
        public string FieldName { get; set; }
        public string DataTable { get; set; }
        public string DataField { get; set; }
    }
}
