using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Globalization;

namespace BPAddIn
{
    public class Dictionary
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

        public Word getWord(string[] wordString)
        {
            string wordS = wordString[0].ToLowerInvariant();
            using (LocalDBContext context = new LocalDBContext())
            {
                var words = from w in context.dictionary
                            where w.word == wordS
                            select w;

                return words.FirstOrDefault<Word>();
            }
        }

        private string getWordFlag(string wordString)
        {
            wordString = wordString.ToLowerInvariant();
            using (LocalDBContext context = new LocalDBContext())
            {
                var words = from w in context.dictionary
                            where w.word == wordString
                            && w.flags.Substring(4, 1) == "1"
                            select w.flags;

                string flags = words.FirstOrDefault<String>();

                return flags;
            }
        }

        public bool isNoun(string[] words)
        {
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

            return false;
        }

        public string getBaseNoun(string wordToConvert)
        {
            string[] splitWord = wordToConvert.Split(' ');
            string word = "";

            if (splitWord.Length == 1)
            {
                word = splitWord[0];
            }
            else if (splitWord.Length > 1)
            {
                word = splitWord[1];
            }
            else
            {
                return "";
            }

            using (LocalDBContext context = new LocalDBContext())
            {
                var words = from w in context.dictionary
                            where w.word == word.ToLower()
                            select w.wordBase;

                try
                {
                    string wordBase = words.FirstOrDefault<String>();

                    if (String.IsNullOrEmpty(wordBase))
                    {
                        MessageBox.Show("Defect could not be corrected.");
                        return "";
                    }

                    if (splitWord.Length == 1)
                    {
                        splitWord[0] = wordBase;
                    }
                    else if (splitWord.Length > 1)
                    {
                        splitWord[1] = wordBase;
                    }
                }
                catch (NullReferenceException)
                {
                }
            }

            splitWord[0] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(splitWord[0].ToLower());

            return String.Join(" ", splitWord);
        }
    }
}
