using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling.Models;

namespace TechExtensions.SchedyBot.BLL.v2.DialogTemplates.Services.AnswerTypeHandling
{
    // TODO: Если сработает, то надо перенести из сервисов в экстеншены
    public static class AnswerTypeGuard
    {
        public static bool IsCorrect<TAnswer>(this AnswerType answerType, TAnswer answer) where TAnswer : class
        {
            switch (answerType)
            {
                case AnswerType.Email:
                    return IsEmailCorrect(answer);
                case AnswerType.Text:
                    return true;
                case AnswerType.Button:
                    return IsButtonAnswerCorrect(answer);
                case AnswerType.Nickname:
                    return IsNicknameCorrect(answer);
                case AnswerType.Phone:
                    return IsPhoneCorrect(answer);
                case AnswerType.Time:
                    return IsTimeCorrect(answer);
                case AnswerType.TimeSpan:
                    return IsTimeSpanCorrect(answer);
                case AnswerType.Empty:
                    return answer == null;
                case AnswerType.IntegralNumber:
                    return IsAnswerIsIntergralNumber(answer);
                case AnswerType.Date:
                    return IsDateCorrect(answer);
                case AnswerType.Name:
                    return IsNameCorrect(answer);
                default:
                    throw new Exception($"Не удалось найти тип ответа из enum для следующего: {answerType.ToString()}");
            }
        }

        private static bool IsTimeSpanCorrect<TAnswer>(TAnswer timeSpan)
        {
            var timeSpanStr = timeSpan.ToString();
            var twoTimes = timeSpanStr.Split('-');
            if (twoTimes.Length < 2)
                return false;
            if ((!IsTimeCorrect(twoTimes[0])) || (!IsTimeCorrect(twoTimes[1])))
                return false;
            return true;
        }
        private static bool IsNameCorrect<TAnswer>(TAnswer name) 
        {
            //какой-то алгоритм 
            return true;
        }

        private static bool IsTimeCorrect<TAnswer>(TAnswer name)
        {
            //какой-то алгоритм 
            var twoNumbers = name.ToString().Split(":");
            if (twoNumbers.Length is 1)
                return false;
            var num = 0;
            if (twoNumbers.Any(s => Int32.TryParse(s, NumberStyles.Integer, null, out num) is false))
                return false;

            var firstNIsValid = Int32.Parse(twoNumbers[0]) <= 23 && 0 <= Int32.Parse(twoNumbers[0]);
            var secondNIsValid = Int32.Parse(twoNumbers[1]) <= 59 && 0 <= Int32.Parse(twoNumbers[1]);
            if ((!firstNIsValid) || (!secondNIsValid))
                return false;
            return true;
        }

        private static bool IsAnswerIsIntergralNumber<TAnswer>(TAnswer money)
        {
            var result = 0;
            if(Int32.TryParse(money.ToString(), out result))
            {
                if (result % 1 == 0)
                    return true;
                return false;
            }
            return false;
        }

        private static bool IsNicknameCorrect<TAnswer>(TAnswer name)
        {
            //какой-то алгоритм 
            return true;
        }

        private static bool IsDateCorrect<TAnswer>(TAnswer dateToCheck)
        {
            string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt",
                         "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss",
                         "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt",
                         "M/d/yyyy h:mm", "M/d/yyyy h:mm",
                         "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm"};
            string[] dateStrings = {"5/1/2009 6:32 PM", "05/01/2009 6:32:05 PM",
                              "5/1/2009 6:32:00", "05/01/2009 06:32",
                              "05/01/2009 06:32:00 PM", "05/01/2009 06:32:00"};
            DateTime dateValue;

            var isDateCorrect = DateTime.TryParseExact(dateToCheck as string, formats,
                new CultureInfo("ru - RU"), DateTimeStyles.None, out dateValue);

            return isDateCorrect;
        }

        private static bool IsPhoneCorrect<TAnswer>(TAnswer phone)
        {
            return Regex.IsMatch(phone as string, @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        private static bool IsEmailCorrect<TAnswer>(TAnswer emailaddress)
        {
            MailAddress email;

            return MailAddress.TryCreate(emailaddress as string, out email);
        }

        /// <summary>
        /// Тоже как пример, но уже более рабочий и демонстрирующий как можно использовать дженерик в наших целях
        /// </summary>
        /// <param name="buttonAnswer"></param>
        /// <returns></returns>
        private static bool IsButtonAnswerCorrect<TAnswer>(TAnswer buttonAnswer)
        {
            var correct = (buttonAnswer as ButtonTypeAnswer).AnswersCollection.Any(a=> a ==  (buttonAnswer as ButtonTypeAnswer).Answer);
            return correct;
        }
       }
    }