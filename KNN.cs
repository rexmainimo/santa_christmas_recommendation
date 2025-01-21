using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;

internal class KNN
{
    private List<GiftRecommendation> products;
    private Dictionary<string, int> allTitlesIndex;
    private Dictionary<string, int> allMainCategoriesIndex;

    public KNN(string filePath)
    {
        products = new List<GiftRecommendation>();
        HashSet<string> allTitlesSet = new HashSet<string>();
        HashSet<string> allMainCategoriesSet = new HashSet<string>();

        string[] file = File.ReadAllLines(filePath);
        for(int i = 1; i < file.Length; i++)
        {
            string cleanedLine = ReplaceCommasWithinQuotes(file[i]);
            string[] clean = cleanedLine.Split(',');
            
            GiftRecommendation product = new GiftRecommendation
            {
                main_category = RemoveSpecialCharacters(clean[0]),
                title = RemoveSpecialCharacters(clean[1]),
            };

            products.Add(product);
            allTitlesSet.UnionWith(Tokenize(product.title));
            allMainCategoriesSet.UnionWith(Tokenize(product.main_category));
        }

        allTitlesIndex = allTitlesSet.Select((word, index) => new { word, index })
                                     .ToDictionary(x => x.word, x => x.index);

        allMainCategoriesIndex = allMainCategoriesSet.Select((word, index) => new { word, index })
                                                     .ToDictionary(x => x.word, x => x.index);
    }

    public void Recommend(string input, bool isPositive)
    {
        double[] userVector = EncodeInput(input);

        List<(GiftRecommendation product, double similarity)> candidates = new List<(GiftRecommendation product, double similarity)>();

        int k = 5;
        foreach (GiftRecommendation product in products)
        {
            double[] productVector = EncodeProduct(product);
            double similarity = CalculateCosineSimilarity(userVector, productVector);
            candidates.Add((product, similarity));
        }
        candidates = candidates.Where(candidate => candidate.similarity > 0.1).ToList();
        if (!candidates.Any())
        {
            Console.WriteLine("No suitable product found.");
            return;
        }

        if (isPositive)
        {
            candidates = candidates.OrderByDescending(candidate => candidate.similarity).ToList();
        }
        else
        {

            candidates = candidates.OrderBy(candidate => candidate.similarity).ToList();
        }

        candidates = candidates.Take(k).ToList();
        (GiftRecommendation product, double similarity) topCandidate = candidates.FirstOrDefault();
        GiftRecommendation recommendedProduct = topCandidate.product;


        if (recommendedProduct != null)
        {

            Console.WriteLine($"{recommendedProduct.main_category.Trim()} | {recommendedProduct.title.Trim()}");
        }
        else
        {
            Console.WriteLine("No suitable product found.");
        }
    }


    private double[] EncodeInput(string input)
    {
        List<string> tokens = Tokenize(input);
        return Vectorize(tokens, allMainCategoriesIndex)
            .Concat(Vectorize(tokens, allTitlesIndex))
            .ToArray();
    }

    private double[] EncodeProduct(GiftRecommendation product)
    {
        List<string> mainCategoryTokens = Tokenize(product.main_category);
        List<string> titleTokens = Tokenize(product.title);

        return Vectorize(mainCategoryTokens, allMainCategoriesIndex)
            .Concat(Vectorize(titleTokens, allTitlesIndex))
            .ToArray();
    }

    private double[] Vectorize(List<string> tokens, Dictionary<string, int> vocabularyIndex)
    {
        double[] vector = new double[vocabularyIndex.Count];
        foreach (string token in tokens)
        {
            if (vocabularyIndex.TryGetValue(token, out int index))
            {
                //The method increments the value in the vector at the position
                //corresponding to each word in the input tokens:
                vector[index]++;
            }
        }
        return vector;
    }

    private List<string> Tokenize(string text)
    {
        HashSet<string> stopwords = new HashSet<string>
        {
            "i", "are", "the", "is", "and", "or", "a", "an", "of", "to", "in", "on",
            "with", "by", "at", "for", "from", "this", "that", "these", "those", "it", "as", "was", "were"
        };

        return text.ToLower()
                   .Split(new[] { ' ', ',', '.', ';', ':', '-', '_', '\"', '\'' }, StringSplitOptions.RemoveEmptyEntries)
                   .Where(word => !stopwords.Contains(word))
                   .ToList();
    }

    private string RemoveSpecialCharacters(string input)
    {
        return Regex.Replace(input.ToLower(), "[^a-z ]", " ").Trim();
    }

    private string ReplaceCommasWithinQuotes(string input)
    {
        bool insideQuotes = false;
        char[] chars = input.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == '\"')
            {
                insideQuotes = !insideQuotes;
            }
            if (insideQuotes && chars[i] == ',')
            {
                chars[i] = ';';
            }
        }

        return new string(chars);
    }

    private double CalculateCosineSimilarity(double[] vectorA, double[] vectorB)
    {
        double dotProduct = 0, magnitudeA = 0, magnitudeB = 0;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        double denominator = Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB);
        if (denominator == 0)
        {
            return 0;
        }
        else
        {
            return dotProduct / denominator;
        }
    }
}

internal class GiftRecommendation
{
    public string main_category { get; set; }
    public string title { get; set; }
}

