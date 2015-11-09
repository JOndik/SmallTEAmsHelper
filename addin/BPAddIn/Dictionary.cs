using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace BPAddIn
{
    class Dictionary
    {
        const char FLAG_NOUN = 'S';
        const char FLAG_ADJECTIVE = 'A';

        public string testSelect()
        {
            string value = "";
            using (LocalDBContext context = new LocalDBContext())
            {
                var words = (from w in context.dictionary
                           select w).Take(1);

                foreach (Word word in words)
                    value = word.word;
            }

            return value;
        }

        private char getWordFlag(string wordString)
        {
            using (LocalDBContext context = new LocalDBContext())
            {
                var words = from w in context.dictionary
                             where w.word == wordString
                             select w.flag;

                try
                {
                    char flag = words.SingleOrDefault<String>()[0];

                    return flag;
                }
                catch (NullReferenceException)
                {
                    return '\0';
                }
            }
        }

        public bool isNoun(string[] words)
        {
            for (int i = 0; i < words.Length; i++)
            {
                if (getWordFlag(words[i]) == FLAG_NOUN)
                {
                    return true;
                }
                else if (i < words.Length - 1 && getWordFlag(words[i]) == FLAG_ADJECTIVE && getWordFlag(words[i + 1]) == FLAG_NOUN)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
