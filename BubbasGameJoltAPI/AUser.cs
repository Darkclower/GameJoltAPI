using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace BubbasGameJoltAPI
{
    public class AUser
    {
        public enum UserType
        {
            User,
            Developer,
            Moderator,
            Admin
        }
        public enum UserStatus
        {
            Active,
            Banned
        }
        public enum UserSessionStatus
        {
            Active,
            Idle
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="error">The error</param>
        public delegate void ErrorDelegate(AUser user, string error);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">The user</param>
        public delegate void CompleteDelegate(AUser user);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">The user who achieved the trophy</param>
        /// <param name="id">The id of the achieved trophy</param>
        /// <param name="alradyAchieved">Whether or not the user have achieved this trophy before (This is completely dependent on the AchievedTrophies list. So if the list isn't up to date, this bool might be incorrect)</param>
        /// <param name="success">If the request was successful</param>
        public delegate void AchieveTrophyComplete(AUser user, string id, bool alradyAchieved);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="trophies">The trophies the user already have achieved</param>
        public delegate void AchievedTrophiesComplete(AUser user, List<Data.RawTrophy> trophies);

        private GameInfo _game;    // The game the user is tied to (All Requests will use the Game's ID and Key)

        private string _token;
        private List<string> _achievedTrophies;

        private UserSessionStatus _sessionStatus;

        private string _id;
        private UserType _type;
        private string _name;
        private string _avatarURL;
        private string _signedUp;
        private string _lastLogin;
        private UserStatus _status;
        private string _developerName;
        private string _developerWebsite;
        private string _developerDescription;

        /// <summary>
        /// The game the user is connected to
        /// </summary>
        public GameInfo Game { get { return _game; } }

        /// <summary>
        /// The Token of the user
        /// </summary>
        public string Token { get { return _token; } set { _token = value; } }
        /// <summary>
        /// The id's of the achieved trophies
        /// </summary>
        public List<string> AchievedTrophies { get { return _achievedTrophies; } set { _achievedTrophies = value; } }

        /// <summary>
        /// Whether or not the user is currently playing
        /// </summary>
        public UserSessionStatus SessionStatus { get { return _sessionStatus; } set { _sessionStatus = value; } }

        /// <summary>
        /// The ID of the user
        /// </summary>
        public string ID { get { return _id; } set { _id = value; } }
        /// <summary>
        /// Can be User, Developer, Moderator, or Admin
        /// </summary>
        public UserType Type { get { return _type; } set { _type = value; } }
        /// <summary>
        /// The user's username
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }
        /// <summary>
        /// The URL of the user's avatar
        /// </summary>
        public string AvatarURL { get { return _avatarURL; } set { _avatarURL = value; } }
        /// <summary>
        /// How long ago the user signed up
        /// </summary>
        public string SignedUp { get { return _signedUp; } set { _signedUp = value; } }
        /// <summary>
        /// How long ago the user was last logged in. Will be "Online Now" if the user is currently online
        /// </summary>
        public string LastLogin { get { return _lastLogin; } set { _lastLogin = value; } }
        /// <summary>
        /// "Active" if the user is still a member on the site. "Banned" if they've been banned
        /// </summary>
        public UserStatus Status { get { return _status; } set { _status = value; } }
        /// <summary>
        /// The Developers name
        /// </summary>
        public string DeveloperName { get { return _developerName; } set { _developerName = value; } }
        /// <summary>
        /// The developers website, if they have one
        /// </summary>
        public string DeveloperWebsite { get { return _developerWebsite; } set { _developerWebsite = value; } }
        /// <summary>
        /// The developers description of themselves. Formatting is removed from this result.
        /// </summary>
        public string DeveloperDescription { get { return _developerDescription; } set { _developerDescription = value; } }


        /// <summary>
        /// Is called whenever an authencation is successful
        /// </summary>
        public AUserRequestComplete OnAuthencationComplete;
        /// <summary>
        /// Is called whenever an authencation failed
        /// </summary>
        public RequestFailed OnAuthencationError;

        /// <summary>
        /// Is called whenever an fetch is successful
        /// </summary>
        public AUserRequestComplete OnFetchComplete;
        /// <summary>
        /// Is called whenever an fetch failed
        /// </summary>
        public ErrorDelegate OnFetchError;

        public CompleteDelegate OnSessionOpenComplete;
        public ErrorDelegate OnSessionOpenError;

        public CompleteDelegate OnSessionPingComplete;
        public ErrorDelegate OnSessionPingError;

        public CompleteDelegate OnSessionCloseComplete;
        public ErrorDelegate OnSessionCloseError;

        public AchievedTrophiesComplete OnAchievedTrophiesComplete;
        public ErrorDelegate OnAchievedTrophiesError;

        public AchieveTrophyComplete OnAchieveTrophyComplete;
        public ErrorDelegate OnAchieveTrophyError;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="game">The game the user is "connected to"</param>
        public AUser(GameInfo game)
        {
            _game = game;
        }

        /// <summary>
        /// Authenticates the user using "Name" and "Token"
        /// Authencating a user means checking if the Username/Token combination is correct
        /// </summary>
        public void Authenticate()
        {
            // Authenticate
            Networking.AuthenticateUser(_game.GameID, _game.GameKey, CallAuthencationComplete, _name, _token);
        }

        /// <summary>
        /// Fetches the user using "Name"
        /// Fetching a user means getting all public details about a user
        /// </summary>
        public void Fetch()
        {
            Networking.FetchUser(_game.GameID, _game.GameKey, CallFetchComplete, _name);
        }

        /// <summary>
        /// Fetches the user using "ID"
        /// Fetching a user means getting all public details about a user
        /// </summary>
        public void FetchByID()
        {
            Networking.FetchUserByID(_game.GameID, _game.GameKey, CallFetchComplete, ID);
        }

        /// <summary>
        /// Opens a session for the user
        /// 
        /// </summary>
        public void OpenSession()
        {
            Networking.OpenSession(_game.GameID, _game.GameKey, CallOpenSessionComplete, _name, _token);
        }

        public void Ping()
        {
            Networking.PingSession(_game.GameID, _game.GameKey, CallPingSessionComplete, _name, _token, (_sessionStatus == UserSessionStatus.Active) ? "active" : "idle");
        }
        /// <summary>
        /// </summary>
        /// <param name="active">If the user is active or idle. Active = true, Idle = false</param>
        public void Ping(bool active)
        {
            Networking.PingSession(_game.GameID, _game.GameKey, CallPingSessionComplete, _name, _token, (active) ? "active" : "idle");
        }

        public void CloseSession()
        {
            Networking.CloseSession(_game.GameID, _game.GameKey, CallCloseSessionComplete, _name, _token);
        }

        public void FetchAchievedTrophies()
        {
            Networking.FetchAllAchievedTrophies(_game.GameID, _game.GameKey, CallAchievedTrophiesComplete, _name, _token, "achieved");
        }

        public void AchieveTrophy(string id)
        {
            Networking.AchieveTrophy(_game.GameID, _game.GameKey, (IAsyncResult result) => { CallAchieveTrophyComplete(id, result); }, _name, _token, "achieved");
        }

        private void CallAuthencationComplete(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                HttpWebRequest request = (HttpWebRequest)result.AsyncState;

                //Deserialize the data and call the event
                string data = new StreamReader(request.EndGetResponse(result).GetResponseStream()).ReadToEnd();
                Data.SuccessResponse successResult = JsonConvert.DeserializeObject<Data.SuccessResult>(data).Response;

                if (successResult.Success[0] == 't')
                {
                    //
                    if (OnAuthencationComplete != null)
                        OnAuthencationComplete(this);
                }
                else
                {
                    //
                    if (OnAuthencationError != null)
                        OnAuthencationError(successResult.Message);
                }
            }
        }

        private void CallOpenSessionComplete(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;

            //Deserialize the data and call the event
            string data = new StreamReader(request.EndGetResponse(result).GetResponseStream()).ReadToEnd();
            Data.SuccessResponse successResult = JsonConvert.DeserializeObject<Data.SuccessResult>(data).Response;

            //
            if (successResult.Success[0] == 't')
            {
                if (OnSessionOpenComplete != null)
                    OnSessionOpenComplete(this);
            }
            else
            {
                if (OnSessionOpenError != null)
                    OnSessionOpenError(this, successResult.Message);
            }
        }

        private void CallPingSessionComplete(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;

            //Deserialize the data and call the event
            string data = new StreamReader(request.EndGetResponse(result).GetResponseStream()).ReadToEnd();
            Data.SuccessResponse successResult = JsonConvert.DeserializeObject<Data.SuccessResult>(data).Response;

            //
            if (successResult.Success[0] == 't')
            {
                if (OnSessionPingComplete != null)
                    OnSessionPingComplete(this);
            }
            else
            {
                if (OnSessionPingError != null)
                    OnSessionPingError(this, successResult.Message);
            }
        }

        private void CallCloseSessionComplete(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;

            //Deserialize the data and call the event
            string data = new StreamReader(request.EndGetResponse(result).GetResponseStream()).ReadToEnd();
            Data.SuccessResponse successResult = JsonConvert.DeserializeObject<Data.SuccessResult>(data).Response;

            //
            if (successResult.Success[0] == 't')
            {
                if (OnSessionCloseComplete != null)
                    OnSessionCloseComplete(this);
            }
            else
            {
                if (OnSessionCloseError != null)
                    OnSessionCloseError(this, successResult.Message);
            }
        }

        private void CallFetchComplete(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                HttpWebRequest request = (HttpWebRequest)result.AsyncState;

                //Deserialize the data and call the event
                string data = new StreamReader(request.EndGetResponse(result).GetResponseStream()).ReadToEnd();
                Data.UserResult userResult = JsonConvert.DeserializeObject<Data.UserResult>(data);

                if (userResult.Response.Success[0] == 't')
                {
                    Data.RawUser user = userResult.Response.Users[0];

                    // Copying values of the same type
                    _id = user.ID;
                    _name = user.Name;
                    _avatarURL = user.AvatarURL;
                    _signedUp = user.SignedUp;
                    _lastLogin = user.LastLogin;
                    _developerName = user.DeveloperName;
                    _developerWebsite = user.DeveloperWebsite;
                    _developerDescription = user.DeveloperDescription;

                    // 
                    if (user.Type[0] == 'A')
                        _type = UserType.Admin;
                    else if (user.Type[0] == 'M')
                        _type = UserType.Moderator;
                    else if (user.Type[0] == 'D')
                        _type = UserType.Developer;
                    else
                        _type = UserType.User;

                    //
                    if (user.Status[0] == 'B')
                        _status = UserStatus.Banned;
                    else
                        _status = UserStatus.Active;

                    //
                    if (OnFetchComplete != null)
                        OnFetchComplete(this);
                }
                else
                {
                    //
                    Data.SuccessResponse success = JsonConvert.DeserializeObject<Data.SuccessResult>(data).Response;

                    //
                    if (OnFetchError != null)
                        OnFetchError(this, success.Message);
                }
            }
        }

        private void CallAchievedTrophiesComplete(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                HttpWebRequest request = (HttpWebRequest)result.AsyncState;

                //Deserialize the data and call the event
                string data = new StreamReader(request.EndGetResponse(result).GetResponseStream()).ReadToEnd();
                Data.TrophyResponse trophyResult = JsonConvert.DeserializeObject<Data.TrophyResult>(data).Response;

                if (trophyResult.Success[0] == 't')
                {
                    List<Data.RawTrophy> trophies = trophyResult.Trophies;

                    if (trophies != null)
                    {
                        //
                        _achievedTrophies = new List<string>();

                        int length = trophies.Count;
                        for (int i = 0; i < length; i++)
                        {
                            AchievedTrophies.Add(trophies[i].ID);
                        }
                    }

                    //
                    if (OnAchievedTrophiesComplete != null)
                        OnAchievedTrophiesComplete(this, trophies);
                }
                else
                {
                    //
                    Data.SuccessResponse successResult = JsonConvert.DeserializeObject<Data.SuccessResult>(data).Response;

                    //
                    if (OnAchievedTrophiesComplete != null)
                        OnAchievedTrophiesError(this, successResult.Message);
                }
            }
        }

        private void CallAchieveTrophyComplete(string id, IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                HttpWebRequest request = (HttpWebRequest)result.AsyncState;

                //Deserialize the data and call the event
                string data = new StreamReader(request.EndGetResponse(result).GetResponseStream()).ReadToEnd();
                Data.SuccessResponse successResult = JsonConvert.DeserializeObject<Data.SuccessResult>(data).Response;

                if (successResult.Success[0] == 't')
                {
                    //
                    bool alreadyAchieved = false;
                    if (_achievedTrophies == null)
                    {
                        _achievedTrophies = new List<string>();
                        _achievedTrophies.Add(id);
                    }
                    else
                    {

                        int length = _achievedTrophies.Count;
                        for (int i = 0; i < length; i++)
                            if (_achievedTrophies[i] == id)
                            {
                                alreadyAchieved = true;
                                break;
                            }

                        if (!alreadyAchieved)
                            _achievedTrophies.Add(id);
                    }

                    //
                    if (OnAchieveTrophyComplete != null)
                        OnAchieveTrophyComplete(this, id, alreadyAchieved);
                }
                else
                {
                    //
                    if (OnAchievedTrophiesError != null)
                        OnAchievedTrophiesError(this, successResult.Message);
                }
            }
        }

        /// <summary>
        /// This holds a value that is true if the user has values for the developer properties and is false otherwise.
        /// </summary>
        public bool IsDeveloper()
        {
            return DeveloperDescription != null || DeveloperWebsite != null || DeveloperName != null;
        }

        /// <summary>
        /// Gets the user's "display name" which is the name that is shown in the chat
        /// For developers, it's the "developer name"
        /// And for the non-developers it's the "name"
        /// </summary>
        /// <returns>The "shown" name of the user</returns>
        public string GetDisplayName()
        {
            if (IsDeveloper())
                return _developerName;
            return _name;
        }

        public override string ToString()
        {
            string ret = "";

            // ID
            if (_id != null)
                ret += "ID:" + _id + "\n";
            // Type (Not nullable, will always be returned)
            ret += "Type:" + _type + "\n";
            // Name
            if (_name != null)
                ret += "Name:" + _name + "\n";
            // AvatarURL
            if (_avatarURL != null)
                ret += "AvatarURL:" + _avatarURL + "\n";
            // SignedUp
            if (_signedUp != null)
                ret += "SignedUp:" + _signedUp + "\n";
            // LastLogin
            if (_lastLogin != null)
                ret += "LastLogin:" + _lastLogin + "\n";
            // Status (Not nullable, will always be returned)
            ret += "Status:" + _status + "\n";
            // DeveloperName
            if (_developerName != null)
                ret += "DeveloperName:" + _developerName + "\n";
            // DeveloperWebsite
            if (_developerWebsite != null)
                ret += "DeveloperWebsite:" + _developerWebsite + "\n";
            // DeveloperDescription
            if (_developerDescription != null)
                ret += "DeveloperDescription:" + _developerDescription + "\n";

            // Removes the last new line symbol
            if (ret != "")
                ret = ret.Remove(ret.Length - 1);

            return ret;
        }
    }
}
