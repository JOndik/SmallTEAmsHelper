using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;

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

        private string getWordFlag(string wordString)
        {
            wordString = wordString.ToLowerInvariant();
            using (LocalDBContext context = new LocalDBContext())
            {
                //MessageBox.Show("hladam pre " + wordString);
                var words = from w in context.dictionary
                             where w.word == wordString
                             && w.flag.Substring(4, 1) == "1"
                             select w.flag;

                /*try
                {*/
                    string flags = words.FirstOrDefault<String>();

                    return flags;
                /*}
                catch (NullReferenceException)
                {
                    return "";
                }*/

            return "";
            }
        }

        public bool isNoun(string[] words)
        {
            /*for (int i = 0; i < words.Length; i++)
            {*/
            //MessageBox.Show(getWordFlag(words[0]));
            if (String.IsNullOrEmpty(getWordFlag(words[0])))
            {
                return false;
            }

                if (words.Length == 1 && getWordFlag(words[0])[0] == FLAG_NOUN && getWordFlag(words[0])[3] == 's')
                {
                    return true;
                }
                else if (words.Length > 1 && (getWordFlag(words[0])[0] == FLAG_ADJECTIVE && getWordFlag(words[1])[0] == FLAG_NOUN && getWordFlag(words[1])[3] == 's')
                    || (getWordFlag(words[0])[0] == FLAG_NOUN && getWordFlag(words[0])[3] == 's'))
                {
                    return true;
                }
            /*}*/

            return false;
        }
    }
}
