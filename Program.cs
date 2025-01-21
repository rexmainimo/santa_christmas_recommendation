using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace santa_christmas_recommendation
{
    internal class Program
    {
        static void Main(string[] args)
        {

            try
            {
                string input = Console.ReadLine().Trim().ToLower();
                bool isNum = int.TryParse(input, out int num);

                if (string.IsNullOrEmpty(input) || isNum || !input.Any(char.IsLetter))
                {
                    Console.WriteLine("Crazy input!");
                    return;
                }
                
                string[] sentiments = File.ReadAllLines("reduced_sentiments_dataset.csv");
                NaiveBayesClassifier nbc = new NaiveBayesClassifier(sentiments);

                bool result = nbc.Predict(input);
                Console.WriteLine("sentiment: " + result);

                string file = "reduced_amazon_dataset.csv";

                KNN knn = new KNN(file);
                knn.Recommend(input, result);


            }
            catch
            {
                Console.WriteLine("Crazy input!");
            }

        }

    }
}
