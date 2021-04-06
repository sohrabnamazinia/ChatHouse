using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Models
{

    public class Interests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Wellness Wellness { get; set; }

        public Identity Identity { get; set; }

        public Places Places { get; set; }

        public WorldAffairs WorldAffairs { get; set; }

        public Tech Tech { get; set; }

        public HangingOut HangingOut { get; set; }

        public KnowLedge KnowLedge { get; set; }

        public Hustle Hustle { get; set; }

        public Sports Sports { get; set; }

        public Arts Arts { get; set; }

        public Life Life { get; set; }

        public Languages Languages { get; set; }

        public Entertainment Entertainment { get; set; }

        public Faith Faith { get; set; }
    }

    public class InterestsService
    {
        public static List<int> ConvertInterestToList<T>(T interests) where T : System.Enum
        {
            List<int> result = new List<int>();
            foreach (T interest in Enum.GetValues(typeof(T)))
                if ((Convert.ToInt32(interests) & Convert.ToInt32(interest)) != 0)
                    result.Add(Convert.ToInt32(interest));

            return result;
        }

        public static int BitWiseOr(List<int> interests)
        {
            var result = 0;
            foreach (var i in interests)
                result |= i;

            return result;
        }

        public static void SetInterests(List<List<int>> interests, AppUser user)
        {
            Wellness wellness = (Wellness)BitWiseOr(interests[0]);
            Identity identity = (Identity)BitWiseOr(interests[1]);
            Places places = (Places)BitWiseOr(interests[2]);
            WorldAffairs worldAffairs = (WorldAffairs)BitWiseOr(interests[3]);
            Tech tech = (Tech)BitWiseOr(interests[4]);
            HangingOut hangingOut = (HangingOut)BitWiseOr(interests[5]);
            KnowLedge knowLedge = (KnowLedge)BitWiseOr(interests[6]);
            Hustle hustle = (Hustle)BitWiseOr(interests[7]);
            Sports sports = (Sports)BitWiseOr(interests[8]);
            Arts arts = (Arts)BitWiseOr(interests[9]);
            Life life = (Life)BitWiseOr(interests[10]);
            Languages languages = (Languages)BitWiseOr(interests[11]);
            Entertainment entertainment = (Entertainment)BitWiseOr(interests[12]);
            Faith faith = (Faith)BitWiseOr(interests[13]);

            user.Interests.Wellness = wellness;
            user.Interests.Identity = identity;
            user.Interests.Places = places;
            user.Interests.WorldAffairs = worldAffairs;
            user.Interests.Tech = tech;
            user.Interests.HangingOut = hangingOut;
            user.Interests.KnowLedge = knowLedge;
            user.Interests.Hustle = hustle;
            user.Interests.Sports = sports;
            user.Interests.Arts = arts;
            user.Interests.Life = life;
            user.Interests.Languages = languages;
            user.Interests.Entertainment = entertainment;
            user.Interests.Faith = faith;
        }

        public static List<List<int>> ConvertInterestsToLists(Interests interest)
        {
            var result = new List<List<int>>();
            
            result.Add(ConvertInterestToList(interest.Wellness));
            result.Add(ConvertInterestToList(interest.Identity));
            result.Add(ConvertInterestToList(interest.Places));
            result.Add(ConvertInterestToList(interest.WorldAffairs));
            result.Add(ConvertInterestToList(interest.Tech));
            result.Add(ConvertInterestToList(interest.HangingOut));
            result.Add(ConvertInterestToList(interest.KnowLedge));
            result.Add(ConvertInterestToList(interest.Hustle));
            result.Add(ConvertInterestToList(interest.Sports));
            result.Add(ConvertInterestToList(interest.Arts));
            result.Add(ConvertInterestToList(interest.Life));
            result.Add(ConvertInterestToList(interest.Languages));
            result.Add(ConvertInterestToList(interest.Entertainment));
            result.Add(ConvertInterestToList(interest.Faith));

            return result;

        }
    }

    [Flags]
    public enum Wellness
    {
        Nutrition = 1,
        Outdoors = (2 << 0),
        Weights = (2 << 1),
        Veganism = (2 << 2),
        Meditation = (2 << 3),
        Fitness = (2 << 4),
        Health = (2 << 5),
        Psychedelics = (2 << 6),
        Mindfulness = (2 << 7),
        Medicine = (2 << 8)
    }

    [Flags]
    public enum Identity
    {
        Women,
        BIPOC = 1,
        Black = (2 << 0),
        BabyBoomers = (2 << 1),
        Latino = (2 << 2),
        Disabled = (2 << 3),
        SouthAsian = (2 << 4),
        GenX = (2 << 5),
        Indigenous = (2 << 6),
        EastAsian = (2 << 7),
        GenZ = (2 << 8),
        LGBTQ = (2 << 9),
        Millennials = (2 << 10)
    }

    [Flags]
    public enum Places
    {
        Paris = 1,
        London = (2 << 0),
        LosAngeles = (2 << 1),
        Nigeria = (2 << 2),
        Africa = (2 << 3),
        Atlanta = (2 << 4),
        India = (2 << 5),
        SanFrancisco = (2 << 6),
        NewYork = (2 << 7)
    }

    [Flags]
    public enum WorldAffairs
    {
        SocialIssues = 1,
        Climate = (2 << 0),
        CurrentEvents = (2 << 1),
        Economics = (2 << 2),
        Markets = (2 << 3),
        Geopolitics = (2 << 4),
        US_Politics = (2 << 5)
    }

    [Flags]
    public enum Tech
    {
        AI = 1,
        DTC = (2 << 0),
        SaaS = (2 << 1),
        Startups = (2 << 2),
        Engineering = (2 << 3),
        AngelInvesting = (2 << 4),
        VentureCapital = (2 << 5),
        Product = (2 << 6),
        Crypto = (2 << 7),
        Marketing = (2 << 8),
        VR_AR = (2 << 9)
    }

    [Flags]
    public enum HangingOut
    {
        Coworking = 1,
        Welcome_Newbies = (2 << 0),
        LateNight = (2 << 1),
        Bring_A_Drink = (2 << 2),
        MeetPeople = (2 << 3),
        PTR = (2 << 4),
        ChillVibes = (2 << 5),
        wtf = (2 << 6)
    }

    [Flags]
    public enum KnowLedge
    {
        Education = 1,
        Physics = (2 << 0),
        Science = (2 << 1),
        Psychology = (2 << 2),
        Math = (2 << 3),
        Biology = (2 << 4),
        TheFuture = (2 << 5),
        Philosophy = (2 << 6),
        History = (2 << 7),
        Space = (2 << 8),
        Covid_19 = (2 << 9)
    }

    [Flags]
    public enum Hustle
    {
        ClubHouse = 1,
        PitchPractice = (2 << 0),
        Entrepreneurship = (2 << 1),
        Stocks = (2 << 2),
        RealEstate = (2 << 3),
        TikTok = (2 << 4),
        Networking = (2 << 5),
        SmallBusiness = (2 << 6),
        Instagram = (2 << 7)
    }

    [Flags]
    public enum Sports
    {
        Basketball = 1,
        Football = (2 << 0),
        Golf = (2 << 1),
        MMA = (2 << 2),
        Cycling = (2 << 3),
        Baseball = (2 << 4),
        Cricket = (2 << 5),
        Tennis = (2 << 6),
        Soccer = (2 << 7),
        Formula1 = (2 << 8)
    }

    [Flags]
    public enum Arts
    {
        Art = 1,
        Food_Drink = (2 << 0),
        Design = (2 << 1),
        Books = (2 << 2),
        Advertising = (2 << 3),
        Dance = (2 << 4),
        Photography = (2 << 5),
        Sci_Fi = (2 << 6),
        Architecture = (2 << 7),
        Beauty = (2 << 8),
        Theater = (2 << 9),
        Fashion = (2 << 10),
        Writing = (2 << 11),
        BurningMan = (2 << 12)
    }

    [Flags]
    public enum Life
    {
        Grownuping = 1,
        Support = (2 << 0),
        Traveling = (2 << 1),
        Weddings = (2 << 2),
        Pregnancy = (2 << 3),
        Relationships = (2 << 4),
        Dating = (2 << 5),
        Parenting = (2 << 6)
    }

    [Flags]
    public enum Languages
    {
        Russian = 1,
        Hindi = (2 << 0),
        Indonesian = (2 << 1),
        German = (2 << 2),
        Spanish = (2 << 3),
        Arabic = (2 << 4),
        Portuguese = (2 << 5),
        French = (2 << 6),
        Mandarin = (2 << 7),
        Japanese = (2 << 8)
    }

    [Flags]
    public enum Entertainment
    {
        Performance = 1,
        Music = (2 << 0),
        Television = (2 << 1),
        Podcasts = (2 << 2),
        Gaming = (2 << 3),
        Fun = (2 << 4),
        Karaoke = (2 << 5),
        Variety = (2 << 6),
        Advice = (2 << 7),
        Anime_and_Manga = (2 << 8),
        Movies = (2 << 9),
        Trivia = (2 << 10),
        Celebrities = (2 << 11),
        Storytelling = (2 << 12),
        Comedy = (2 << 13)
    }

    [Flags]
    public enum Faith
    {
        Islam = 1,
        Buddhism = (2 << 0),
        Hinduism = (2 << 1),
        Christianity = (2 << 2),
        Sprituality = (2 << 3),
        Taoism = (2 << 4),
        Atheism = (2 << 5),
        Sikhism = (2 << 6),
        Judaism = (2 << 7),
        Agnosticism = (2 << 8)
    }
}
