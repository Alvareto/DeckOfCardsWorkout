using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Deck_of_Cards_Workout
{
    static class Program
    {
        static void Main(string[] args)
        {
            DeckOfCards.Settings.NUMBER_OF_CARDS = 5;

            Play();
        }

        static void Play()
        {
            while (DeckOfCards.Play())
            {
                Console.WriteLine(DeckOfCards.Statistics.Current.Cards.CardsToString());
                Console.Write("Number of reps: " + DeckOfCards.Statistics.Current.Value);
                Console.ReadLine();
            }

            Console.WriteLine("Total number of reps: " + DeckOfCards.Statistics.TOTAL_REPS);
            Console.ReadLine();
        }
    }

    public static class DeckOfCards
    {
        public static class Settings
        {
            private const int DECK_COUNT = 52;
            public const int CARD_COUNT = 4;
            public const int ACE_VALUE = 1;
            public const int FACE_CARD_VALUE = 10;
            public static int NUMBER_OF_CARDS = 5;

        }

        public static class Statistics
        {
            public static int TOTAL_REPS = 0;

            public static class Current
            {
                public static List<Card> Cards = new List<Card>();
                public static Int32 Value = 0;
            }
        }


        public static Boolean Play()
        {
            if (!HasCards) return false;

            var t = Choose(Settings.NUMBER_OF_CARDS);
            Statistics.Current.Cards = t.Item1;
            Statistics.Current.Value = t.Item2;
            Statistics.TOTAL_REPS += Statistics.Current.Value;

            return true;
        }

        private readonly static Dictionary<Card, Int32> Cards;

        private static Boolean HasCards
        {
            get { return Cards.Count > Settings.NUMBER_OF_CARDS; }
        }

        static DeckOfCards()
        {
            Cards = new Dictionary<Card, int>()
            {
                {Card.Two, Settings.CARD_COUNT},
                {Card.Three, Settings.CARD_COUNT},
                {Card.Four, Settings.CARD_COUNT},
                {Card.Five, Settings.CARD_COUNT},
                {Card.Six, Settings.CARD_COUNT},
                {Card.Seven, Settings.CARD_COUNT},
                {Card.Eight, Settings.CARD_COUNT},
                {Card.Nine, Settings.CARD_COUNT},
                {Card.Ten, Settings.CARD_COUNT},
                {Card.Jack, Settings.CARD_COUNT},
                {Card.Queen, Settings.CARD_COUNT},
                {Card.King, Settings.CARD_COUNT},
                {Card.Ace, Settings.CARD_COUNT}
            };


            //Color.AddRange(new[] { Card.Two, Card.Three, Card.Four, Card.Five, Card.Six, Card.Seven, Card.Eight, Card.Nine, Card.Ten, Card.Jack, Card.Queen, Card.King, Card.Ace });
            //Cards.AddRangeNTimes(Color, Settings.CARD_COUNT);
        }

        //public static List<T> AddRangeNTimes<T>(this List<T> list, IEnumerable<T> collection, UInt32 n)
        //{
        //    for (int i = 0; i < n; i++)
        //    {
        //        list.AddRange(collection);
        //    }
        //    return list;
        //}

        private static IEnumerable<TKey> RandomValues<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict.Keys.ToList().Shuffle();
            //Random rand = new Random();
            //List<TKey> keys = Enumerable.ToList(dict.Keys);
            //int size = dict.Count - 1;
            //while (true)
            //{
            //    yield return keys[rand.Next(size)];
            //}
        }

        private static IList<T> Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public enum Card
        {
            [Description("2")]
            Two = 2,
            [Description("3")]
            Three = 3,
            [Description("4")]
            Four = 4,
            [Description("5")]
            Five = 5,
            [Description("6")]
            Six = 6,
            [Description("7")]
            Seven = 7,
            [Description("8")]
            Eight = 8,
            [Description("9")]
            Nine = 9,
            [Description("10")]
            Ten = 10,
            [Description("J")]
            Jack = Settings.FACE_CARD_VALUE + 1,
            [Description("Q")]
            Queen = Settings.FACE_CARD_VALUE + 2,
            [Description("K")]
            King = Settings.FACE_CARD_VALUE + 3,
            [Description("A")]
            Ace = Settings.ACE_VALUE
        }

        private static IDictionary<TKey, int> ReduceOrRemove<TKey>(this IDictionary<TKey, int> dict, List<TKey> keys)
        {
            foreach (var key in keys)
            {
                if (dict.ContainsKey(key))
                    if (dict[key] != 0)
                        dict[key]--;
                    else dict.Remove(key);
            }

            return dict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numOfCards"></param>
        /// <returns>List of cards and sum of cards</returns>
        private static Tuple<List<Card>, int> Choose(int numOfCards)
        {
            Settings.NUMBER_OF_CARDS = numOfCards;

            // choose random, 
            var chosenCards = Cards.RandomValues().Take(Settings.NUMBER_OF_CARDS).ToList();
            Cards.ReduceOrRemove(chosenCards);

            return new Tuple<List<Card>, int>(chosenCards, chosenCards.Sum(card => (int)card));
        }

        internal static String CardsToString(this IEnumerable<Card> cards)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Selected cards are:").AppendLine();
            foreach (var card in cards)
            {
                sb.Append(card.GetEnumDescription()).AppendLine();
            }

            return sb.ToString();
        }


    }

    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
