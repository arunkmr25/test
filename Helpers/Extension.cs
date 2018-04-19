using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace connections.Helpers
{
    public static class Extension{
        public static void ApplicationError(this HttpResponse response, string message){
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers","Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin","*");                        
        }
         public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalPage, int totalItems){
            var paginationHeader = new PaginationHeaders(currentPage ,itemsPerPage ,totalPage ,totalItems);
            
            var camelCaseSerializer = new JsonSerializerSettings();
            camelCaseSerializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination",JsonConvert.SerializeObject(paginationHeader,camelCaseSerializer));     
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");                   
        }
        

         public static int CalculateAge(this DateTime date){
            var age= DateTime.Today.Year - date.Year;
            if(date.AddYears(age)>DateTime.Today)
            {
                age--;
            }
            return age;
        }
    }
}