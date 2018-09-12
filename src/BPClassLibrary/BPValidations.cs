using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// BPValidations class library has some formatting methods for Capitalising names
/// and Formatting the phone numbers
/// </summary>
namespace BPClassLibrary
{
    public class BPValidations
    {
        public static string Capitalise(string word)
        {
            string[] wordSplit;
            word = word.ToLower();
            word = word.Trim();

            wordSplit = word.Split(' ');

            foreach (string s in wordSplit)
            {
                word = char.ToUpper(s[0]) + s.Substring(1);
            }

            return word;
        }

        public static string FormatPhoneNumber(string phone)
        {
            if (phone[3] != '-')
            {
                phone = phone.Insert(3, "-");
            }
            if (phone[7] != '-')
            {
                phone = phone.Insert(7, "-");
            }

            return phone;
        }
    }
}
