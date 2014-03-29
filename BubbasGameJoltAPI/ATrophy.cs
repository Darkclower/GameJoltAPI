using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BubbasGameJoltAPI
{
    public class ATrophy
    {
        public enum TrophyDifficulty
        {
            Bronze,
            Silver,
            Gold,
            Platinum
        }

        private string _id;
        private string _title;
        private string _description;
        private TrophyDifficulty _difficulty;
        private string _imageURL;

        public string ID { get { return _id; } set { _id = value; } }
        public string Title { get { return _title; } set { _title = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public TrophyDifficulty Difficulty { get { return _difficulty; } set { _difficulty = value; } }
        public string ImageURL { get { return _imageURL; } set { _imageURL = value; } }

        public ATrophy()
        {

        }
        public ATrophy(Data.RawTrophy trophy)
        {
            _id = trophy.ID;
            _title = trophy.Title;
            _description = trophy.Description;
            _imageURL = trophy.ImageURL;

            switch (trophy.Difficulty[0])
            {
                default: // Bronze
                    _difficulty = TrophyDifficulty.Bronze;
                    break;
                case 's': // Silver
                    _difficulty = TrophyDifficulty.Silver;
                    break;
                case 'g': // Gold
                    _difficulty = TrophyDifficulty.Gold;
                    break;
                case 'p': // Platinum
                    _difficulty = TrophyDifficulty.Platinum;
                    break;
            }
        }
    }
}
