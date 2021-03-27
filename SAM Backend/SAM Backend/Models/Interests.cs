using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Models
{
    public class Interests
    {
        public enum Wellness
        {
            Nutrition,
            Outdoors,
            Weights,
            Veganism,
            Meditation,
            Fitness,
            Health,
            Psychedelics,
            Mindfulness,
            Medicine
        }

        public enum Identity
        {
            Women,
            BIPOC,
            Black,
            BabyBoomers,
            Latino,
            Disabled,
            SouthAsian,
            GenX,
            Indigenous,
            EastAsian,
            GenZ,
            LGBTQ,
            Millennials
        }

        public enum Places
        {
            Paris,
            London,
            LosAngeles,
            Nigeria,
            Africa,
            Atlanta,
            India,
            SanFrancisco,
            NewYork
        }

        public enum WorldAffairs
        {
            SocialIssues,
            Climate,
            CurrentEvents,
            Economics,
            Markets,
            Geopolitics,
            US_Politics
        }

        public enum Tech
        {
            AI,
            DTC,
            SaaS,
            Startups,
            Engineering,
            AngelInvesting,
            VentureCapital,
            Product,
            Crypto,
            Marketing,
            VR_AR
        }

        public enum HangingOut
        {
            Coworking,
            Welcome_Newbies,
            LateNight,
            Bring_A_Drink,
            MeetPeople,
            PTR,
            ChillVibes,
            wtf
        }

        public enum KnowLedge
        {
            Education,
            Physics,
            Science,
            Psychology,
            Math,
            Biology,
            TheFuture,
            Philosophy,
            History,
            Space,
            Covid_19
        }

        public enum Hustle
        {
            ClubHouse,
            PitchPractice,
            Entrepreneurship,
            Stocks,
            RealEstate,
            TikTok,
            Networking,
            SmallBusiness,
            Instagram
        }

        public enum Sports
        {
            Basketball,
            Football,
            Golf,
            MMA,
            Cycling,
            Baseball,
            Cricket,
            Tennis,
            Soccer,
            Formula1
        }
        
        public enum Arts
        {
            Art,
            Food_Drink,
            Design,
            Books,
            Advertising,
            Dance,
            Photography,
            Sci_Fi,
            Architecture,
            Beauty,
            Theater,
            Fashion,
            Writing,
            BurningMan
        }

        public enum Life
        {
            Grownuping,
            Support,
            Traveling,
            Weddings,
            Pregnancy,
            Relationships,
            Dating,
            Parenting
        }

        public enum Languages
        {
            Russian,
            Hindi,
            Indonesian,
            German,
            Spanish,
            Arabic,
            Portuguese,
            French,
            Mandarin,
            Japanese
        }

        public enum Entertainment
        {
            Performance,
            Music,
            Television,
            Podcasts,
            Gaming,
            Fun,
            Karaoke,
            Variety,
            Advice,
            Anime_and_Manga,
            Movies,
            Trivia,
            Celebrities,
            Storytelling,
            Comedy
        }

        public enum Faith
        {
            Islam,
            Buddhism,
            Hinduism,
            Christianity,
            Sprituality,
            Taoism,
            Atheism,
            Sikhism,
            Judaism,
            Angosticism
        }
    }
}
