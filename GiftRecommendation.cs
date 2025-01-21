using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace santa_christmas_recommendation
{
    public class SentimentBasedGiftRecommender
    {
        private List<GiftRecommendation> productList;
        public void LoadData(string filePath)
        {

            productList = new List<GiftRecommendation>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                productList = csv.GetRecords<GiftRecommendation>().ToList();
            }
            for(int i = 0; i < 5; i++)
            {
                Console.WriteLine(productList[i].title);
            }
            productList = new List<GiftRecommendation>();
            /*for (int i = 1; i < products.Length; i++)
            {
                string[] line = products[i].Split(',');
                Console.WriteLine(line[2]);
                productList.Add(new GiftRecommendation
                {

                    MainCategory = line[0].ToLower(),
                    Title = line[1].ToLower(),
                    Features = line[4].ToLower(),
                    Description = line[5].ToLower()
                });
            }*/

        }

       /* public List<GiftRecommendation> RecommendGifts(string userInput, bool sentiment)
        {
            
            string inputLowerCase = userInput.ToLower();

            
            List<GiftRecommendation> allTitlesSet = new List<GiftRecommendation>();

            foreach (var product in productList)
            {
                
                double matchScore = CalculateMatchScore(inputLowerCase, product);

                if (sentiment && matchScore > 0.5)  
                {
                    Console.Write(matchScore + " ");
                    recommendedGifts.Add(product);
                }
                else if (!sentiment && matchScore < 0.2)
                {
                    Console.Write(matchScore + " ");
                    recommendedGifts.Add(product);
                }
            }

            return recommendedGifts;
        }
        private double CalculateMatchScore(string userInput, GiftRecommendation product)
        {
            int matchCount = 0;
            //Dictionary<string, int> matched = new Dictionary<string, int>();
            string[] inputWords = userInput.Split(' ');

            
            string[] productKeywords = (product.Title + " " + product.Features + " " + product.Description).ToLower().Split(' ');

            for (int i = 0; i < inputWords.Length; i++)
            {
                if (productKeywords.Contains(inputWords[i]))
                {
                    matchCount++;

                }
            }

            return (double)matchCount / inputWords.Length;
        }*/
    }
}
