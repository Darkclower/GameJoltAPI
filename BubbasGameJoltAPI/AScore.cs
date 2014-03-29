using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BubbasGameJoltAPI
{
    public class AScore
    {
        private string _score;
        private string _sort;
        private string _extraData;
        private string _user;
        private string _userID;
        private string _guest;
        private string _stored;

        /// <summary>
        /// This is a string value associated with the score. Example: "234 Jumps".
        /// </summary>
        public string Score { get { return _score; } set { _score = value; } }
        /// <summary>
        /// This is a numerical sorting value associated with the score. All sorting will work off of this number. Example: "234".
        /// </summary>
        public string Sort { get { return _sort; } set { _sort = value; } }
        /// <summary>
        /// If there's any extra data you would like to store (as a string), you can use this variable. Note that this is only retrievable through the API. It never shows on the actual site.
        /// </summary>
        public string ExtraData { get { return _extraData; } set { _extraData = value; } }
        /// <summary>
        /// The username of the user. Blank if you're storing for a guest.
        /// </summary>
        public string User { get { return _user; } set { _user = value; } }
        /// <summary>
        /// If this is a user score, this is the user's ID.
        /// </summary>
        public string UserID { get { return _userID; } set { _userID = value; } }
        /// <summary>
        /// The guest's name. Blank if you're storing for a user.
        /// </summary>
        public string Guest { get { return _guest; } set { _guest = value; } }
        /// <summary>
        /// Returns when the score was logged by the user.
        /// </summary>
        public string Stored { get { return _stored; } set { _stored = value; } }

        public AScore(Data.RawScore score)
        {
            // Copy values
            _score = score.Score;
            _sort = score.Sort;
            _extraData = score.ExtraData;
            _user = score.User;
            _userID = score.UserID;
            _guest = score.Guest;
            _stored = score.Stored;
        }
    }
}
