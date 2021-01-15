using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TankGame.Src.Data.Statistics
{
    internal static class ScoreManager
    {
        private static readonly string statistics = "Resources/Statistics/Statistics.xml";

        public static void AddScore(string name, long points)
        {
            if (name == null || name.Length == 0) name = "unnamed";

            try
            {
                XmlDocument scores = new XmlDocument();
                scores.Load(statistics);

                XmlElement newScoreElement = scores.CreateElement("score");
                XmlElement nameElement = scores.CreateElement("name");
                XmlElement pointsElement = scores.CreateElement("points");

                nameElement.InnerText = name;
                pointsElement.InnerText = points.ToString();

                newScoreElement.AppendChild(nameElement);
                newScoreElement.AppendChild(pointsElement);

                scores.DocumentElement.AppendChild(newScoreElement);
                scores.Save(statistics);
            }
            catch (Exception)
            {
                XmlDocument scores = new XmlDocument();
                scores.CreateXmlDeclaration("1.0", "utf-8", null);
                scores.AppendChild(scores.CreateElement("statistics"));
                scores.Save(statistics);
            }
        }

        public static List<Tuple<string, string>> GetScores(int count, int offset)
        {
            XDocument scores = XDocument.Load(statistics);

            if (scores != null)
            {
                try
                {
                    return (from score in scores.Root.Descendants("score") select new Tuple<string, string>(score.Element("name").Value, score.Element("points").Value))
                        .OrderByDescending(score => long.Parse(score.Item2))
                        .Skip(offset)
                        .Take(count)
                        .ToList();
                }
                catch (Exception)
                {
                    return new List<Tuple<string, string>>();
                }
            }
            else return new List<Tuple<string, string>>();
        }

        public static int GetScoresCount()
        {
            XDocument scores = XDocument.Load(statistics);

            if (scores?.Root?.Descendants("score") != null)
            {
                return scores.Root.Descendants("score").Count();
            }
            else return 0;
        }
    }
}