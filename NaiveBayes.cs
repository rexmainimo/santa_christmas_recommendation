using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace santa_christmas_recommendation
{
    internal class NaiveBayesClassifier
    {
        public double Negative;
        public double Positive;
        public double Total;
        private Dictionary<string, int> positiveWordCounts;
        private Dictionary<string, int> negativeWordCounts;
        private int totalPositiveWords;
        private int totalNegativeWords;
        private HashSet<string> vocabulary;

        private readonly HashSet<string> StopWords = new HashSet<string>
        {
            "i", "are", "the", "is", "and", "or", "a", "an", "of", "to", "in", "on", "with", "by", "at", "for", "from", "this", "that", "these"
        };
        public NaiveBayesClassifier(string[] file)
        {
            double negCount = 0;
            double posCount = 0;
            positiveWordCounts = new Dictionary<string, int>();
            negativeWordCounts = new Dictionary<string, int>();
            vocabulary = new HashSet<string>();
            
            for (int i = 1; i < file.Length; i++)
            {
                string[] line = file[i].Split(',');
                int sentiment = int.Parse(line[0]);
                string text = line[1].ToLower();
                string[] words = RemoveStopWords(FilterWords(text));
                if (sentiment == 0)
                {
                    negCount++;

                    foreach(string word in words)
                    {
                        if (!negativeWordCounts.ContainsKey(word))
                        {
                            negativeWordCounts[word] = 0;
                        }
                        else
                        {
                            negativeWordCounts[word]++;
                            vocabulary.Add(word);

                        }
                    }
                }
                else if (sentiment == 4)
                {
                    posCount++;
                    foreach(string word in words)
                    {
                        if (!positiveWordCounts.ContainsKey(word))
                        {
                            positiveWordCounts[word] = 0;
                        }
                        else
                        {
                            positiveWordCounts[word]++;
                            vocabulary.Add(word);
                        }
                    }
                }

            }
            // calculate the prior probabilities
            Total = posCount + negCount;

            Positive = posCount / Total;
            Negative = negCount / Total;
            
            totalPositiveWords = SumWordCount(positiveWordCounts);
            totalNegativeWords = SumWordCount(negativeWordCounts);

        }
        private string[] FilterWords(string line)
        {
            string s = Regex.Replace(line.ToLower(), "[^a-z]", " ");
           
            return s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
        private int SumWordCount(Dictionary<string, int> dict)
        {
            int sum = 0;
            foreach(var kvp in dict.Values)
            {
                sum += kvp;
            }
            return sum;
        }
        private double GetWordProbability(string word, bool isPositive)
        {
            word = word.ToLower(); 

            if (isPositive)
            {
                int wordCount;
                if (positiveWordCounts.ContainsKey(word))
                {
                    wordCount = positiveWordCounts[word];
                }
                else
                {
                    wordCount = 0; 
                }

                return (double)(wordCount + 1) / (totalPositiveWords + vocabulary.Count);
            }
            else
            {
                int wordCount;
                if (negativeWordCounts.ContainsKey(word))
                {
                    wordCount = negativeWordCounts[word];
                }
                else
                {
                    wordCount = 0; 
                }

                
                return (double)(wordCount + 1) / (totalNegativeWords + vocabulary.Count);
            }
        }

        public bool Predict(string text)
        {
            string[] words = RemoveStopWords(FilterWords(text));
            double positiveScore = Positive;
            double negativeScore = Negative;

            foreach (string word in words)
            {
                
                positiveScore += GetWordProbability(word, true);
                negativeScore += GetWordProbability(word, false);
            }
           
            if(positiveScore > negativeScore)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string[] RemoveStopWords(string[] words)
        {
            return words.Where(word => !StopWords.Contains(word)).ToArray();
        }
        
    }

}
