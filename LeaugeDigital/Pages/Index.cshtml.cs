using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LeaugeDigital.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LeaugeDigital.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        [BindProperty]
        public Person Person { get; set; }
        [BindProperty]
        public string Message { get; set; }
        [BindProperty]
        public List<Person> People { get; set; }
        [BindProperty]
        public bool isReady { get; set; }
        [BindProperty]
        public bool ErrorStatus { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
        public void OnPost()
        {
            if(!string.IsNullOrEmpty(Person.StartDate) && !string.IsNullOrEmpty(Person.EndDate))
            {
          
            var sDate = Person.StartDate.Split('-');
            var eDate = Person.EndDate.Split('-');
            var startDate = new DateTime(int.Parse(sDate[0]), int.Parse(sDate[1]), int.Parse(sDate[2]));
            var endDate = new DateTime(int.Parse(eDate[0]), int.Parse(eDate[1]), int.Parse(eDate[2]));
            int time = (int) endDate.Subtract(startDate).TotalDays;
            if (startDate > endDate)
            {
                ErrorStatus = true;
                Message = "Invalid Entry, Start Date Exceeds End Date. Please Try Again";
                return;
            }
            if(Person.PhoneNo.Length != 10 || Person.PhoneNo.Any(x => char.IsLetter(x)))
            {
                ErrorStatus = true;
                Message = "Invalid Entry, Number has to be 10 Digits. Please Try Again";
                return;
            }
            if (Person != null)
            {
             
                isReady = true;
                var base_path = Directory.GetCurrentDirectory();
                var final_path = base_path + "/wwwroot/objects.json";
                Person.timeDifference = time;
                Person.StartDate = $"{startDate.Day} {FormatDate(startDate)} {startDate.Year}";
                Person.EndDate = $"{endDate.Day} {FormatDate(endDate)} {endDate.Year}";
                var flag = AddData(Person);
                People = loadData(final_path);
                if (flag) {
                        Message = "Record Already exists! Please add new record";
                    }
                    else
                    {
                        Message = "Thank you for completing the form, Record Added successfully!";
                    }
              
             }
            }
            else
            {
                Message = "Error Occured Please try again!";
            }
        }
        private string FormatDate(DateTime s)
        {
            switch (s.Month)
            {
                case 0: return "January";
                case 1: return "Febraury";
                case 2: return "March";
                case 3: return "April";
                case 4: return "May";
                case 5: return "June";
                case 6: return "July";
                case 7: return "August";
                case 8: return "September";
                case 9: return "October";
                case 10: return "November";
                case 11: return "December";
                default: return "Invalid";
            }
        }
        private bool AddData(Person p)
        {
            try
            {
                var flag = true;
                var base_path = Directory.GetCurrentDirectory();
                var final_path = base_path + "/wwwroot/objects.json";
                var data = loadData(final_path);
                if (!data.Any(x => x.Name == p.Name && x.Email == p.Email && x.PhoneNo == p.PhoneNo && x.timeDifference == p.timeDifference))
                {
                    data.Add(p);
                    using (StreamWriter wr = new StreamWriter(final_path))
                    {
                        var result = JsonConvert.SerializeObject(data);
                        wr.WriteLine(result);
                        wr.Close();
                    }
                    flag = false;
                }
       

                return flag;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }


        private List<Person> loadData(string final_path)
        {
            try
            {
                using (StreamReader r = new StreamReader(final_path))
                {
                    string json = r.ReadToEnd();
                    List<Person> items = JsonConvert.DeserializeObject<List<Person>>(json);
                    r.Close();
                    if(items == null) return new List<Person>();

                    return items;

                }

            }
            catch (Exception e)
            {
                return new List<Person>();
            }

        }
    }
}
