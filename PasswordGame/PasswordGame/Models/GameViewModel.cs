using Newtonsoft.Json;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PasswordGame.Models
{
    public class GameViewModel
    {
        public bool IsPasswordComplete { get; set; }
        public int LastRule { get; set; } = 1;
        public string HtmlResult {get; set;}
        
        public List<Rule> Rules => new List<Rule>()
        {
                new Rule("Your password must have more than 5 symbols!",CheckLengthBy5 ),
                new Rule("Your password must have digit!",CheckByDigit),
                new Rule("Your password must have capital letter!",CheckByCapitalLetter),
                new Rule("Your password must have special symbol!",CheckBySpecialSymbol),
                new Rule("Your password must have digits count equal to 25  !",CheckDigitsCount25),
                new Rule("Your password must have month name in English!",CheckMonthInEnglish),
                new Rule("Your password must have current time of Tokyo!",CheckTokyoTime),
        };
        private bool CheckTokyoTime(string password)
        {
            var response = CheckTokyoTimeAsync(password);
            response.Wait();
            return response.Result;
            async Task<bool> CheckTokyoTimeAsync(string password)
            {
                TimeApiResponse apiResponse = new TimeApiResponse();

                using (HttpClient client = new HttpClient())
                {
                    string urlApi = "https://www.timeapi.io/api/Time/current/zone?timeZone=Asia/Tokyo";
                    var response = await client.GetAsync(urlApi);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        apiResponse = JsonConvert.DeserializeObject<TimeApiResponse>(responseString);
                        Console.WriteLine("Time in Tokyo: " + apiResponse.hour);
                    }
                }

                return password.Contains(apiResponse.hour.ToString());

            }
        }
        

        private bool CheckMonthInEnglish(string password)
        {
            return Regex.IsMatch(password, @"(January|February|March|April|May|June|July|August|September|October|November|December)", RegexOptions.IgnoreCase);
        }

        public bool CheckLengthBy5(string password)
        {
            return password.Length>5;
        }
        public bool CheckByDigit(string password)
        {
            return password.Any(char.IsDigit);
        }
        public bool CheckByCapitalLetter(string password)
        {
            return Regex.IsMatch(password, @"[A-Z]");
        }
        public bool CheckBySpecialSymbol(string password)
        {
            return Regex.IsMatch(password, @"[/!@#$%^&*()_+\-=\\{};':""\\|,.<>\/?+/]");
        }
        public bool CheckDigitsCount25(string password)
        {
            var digits = password.Where(char.IsDigit);
            int sum = digits.Sum(c => int.Parse(c.ToString()));
            return  sum == 25;
        }
    }
    public class  Rule
    {
        /// <summary>
        /// This delegate for getting methods for constructor
        /// </summary>
        /// <param name="password">password</param>
        /// <returns>true or false</returns>
        public delegate bool RuleCheck(string password);

        /// <summary>
        /// Info
        /// </summary>
        public string Text {get; set;}
        public RuleCheck Action {get; set;}

        /// <summary>
        /// Contstructor
        /// </summary>
        /// <param name="text">info</param>
        /// <param name="action">action</param>
        public Rule(string text, RuleCheck action)
        {
            this.Text = text;
            this.Action = action;
        }
    }
    public  class TimeApiResponse
    {
        public int hour { get; set;}
    }
}