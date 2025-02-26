using HCP.Service.Services.ListService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.CleaningServiceDTO
{
    public class CleaningServiceListDTO
    {
        public List<CleaningServiceItemDTO> Items {  get; set; }
        public int totalCount {  get; set; }
        public int totalPages { get; set; }
        public bool hasNext {  get; set; }
        public bool hasPrevious { get; set; }
    }
    public class CleaningServiceItemDTO
    {
        public Guid id { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public decimal overallRating { get; set; }
        public decimal price {  get; set; }
        public string location { get; set; }

    }
    public class CategoryDTO
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imgUrl { get; set; }
    }
    public class ImgDTO
    {
        public Guid id { get; set; }
        public string url { get; set; }
    }
    public class housekeeperDetailDTO
    {
        public string id { get; set; }
        public string name { get; set; }
        public string review { get; set; }
        public string avatar { get; set; }
        public string memberSince { get; set; }
        public string address {  get; set; }
        public string email {  get; set; }
        public string mobile { get; set; }  
        public int numOfServices {  get; set; }
    }
    public class AdditionalServicedDTO
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string price { get; set; }
    }
    public class ServiceDetailDTO
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public int numOfBooks {  get; set; }
        public string location { get;set; }
        public decimal reviews {  get; set; }
        public int numOfReviews {  get; set; }
        public int numOfPics { get; set; }
        public string overview { get; set; }
        public List<ImgDTO> images { get; set; }
        public List<string> steps { get; set; }
        public List<AdditionalServicedDTO> additionalServices { get; set; }
        public housekeeperDetailDTO housekeeper {  get; set; }

     }
    
    public class ServiceDetailWithStatusDTO
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public int numOfBooks {  get; set; }
        public string location { get;set; }
        public decimal reviews {  get; set; }
        public int numOfReviews {  get; set; }
        public int numOfPics { get; set; }
        public string overview { get; set; }
        public List<ImgDTO> images { get; set; }
        public List<string> steps { get; set; }
        public List<AdditionalServicedDTO> additionalServices { get; set; }
        public housekeeperDetailDTO housekeeper {  get; set; }

     }

    public class ServiceStatusUpdateDto
    {
        [JsonPropertyName("service_id")]
        public Guid ServiceId { get; set; }

        [JsonPropertyName("is_approve")]
        public bool IsApprove { get; set; }   // True = "active", False = "rejected"
    }

    public class ServiceTimeSlotDTO1
    {
        public Guid Id { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public string DayOfWeek {  get; set; }
    }
}
