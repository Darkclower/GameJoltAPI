using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace BubbasGameJoltAPI
{
    public class AScoreTable
    {
        public delegate void ErrorDelegate(AScoreTable table, string error);
        public delegate void CompleteDelegate(AScoreTable table);

        private GameInfo _game;

        private AScore[] _scores;
        private int _limit;

        private string _id;
        private string _name;
        private string _description;
        private bool _primary;

        public AScore[] Scores { get { return _scores; } }
        public int Limit { get { return _limit; } set { _limit = value; } }

        public string ID { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public bool Primary { get { return _primary; } set { _primary = value; } }

        public CompleteDelegate OnFetchComplete;
        public ErrorDelegate OnFetchError;

        public CompleteDelegate OnAddScoreComplete;
        public ErrorDelegate OnAddScoreError;

        public AScoreTable(GameInfo game)
        {
            _game = game;
        }
        public AScoreTable(GameInfo game, Data.RawScoreTable table)
        {
            _game = game;

            _id = table.ID;
            _name = table.Name;
            _description = table.Description;
            _primary = (table.Primary == "true");
        }

        public void AddScore(string username, string token, string score, string sort)
        {
            Networking.AddScore(_game.GameID, _game.GameKey, CallAddScoreComplete, username, token, score, sort, "", _id);
        }
        public void AddScore(string username, string token, string score, string sort, string extra)
        {
            Networking.AddScore(_game.GameID, _game.GameKey, CallAddScoreComplete, username, token, score, sort, extra);
        }

        public void AddGuestScore(string name, string score, string sort)
        {
            Networking.AddScoreAsGuest(_game.GameID, _game.GameKey, CallAddScoreComplete, name, score, sort);
        }

        public void Fetch()
        {
            // Fetcha
            Networking.FetchScores(_game.GameID, _game.GameKey, CallFetchComplete, _id, _limit.ToString());
        }
        public void Fetch(string username, string token)
        {
            // Fetcha
            Networking.FetchUserScores(_game.GameID, _game.GameKey, CallFetchComplete,username, token, _id, _limit.ToString());
        }

        private void CallAddScoreComplete(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                HttpWebRequest request = (HttpWebRequest)result.AsyncState;

                //Deserialize the data and call the event
                string data = new StreamReader(request.EndGetResponse(result).GetResponseStream()).ReadToEnd();
                Data.SuccessResponse response = JsonConvert.DeserializeObject<Data.SuccessResult>(data).Response;

                if (response.Success[0] == 't')
                {
                    //
                    if (OnAddScoreComplete != null)
                        OnAddScoreComplete(this);
                }
                else
                {
                    //
                    Data.SuccessResponse successResult = JsonConvert.DeserializeObject<Data.SuccessResult>(data).Response;

                    //
                    if (OnAddScoreError != null)
                        OnAddScoreError(this, successResult.Message);
                }
            }
        }
        private void CallFetchComplete(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                HttpWebRequest request = (HttpWebRequest)result.AsyncState;

                //Deserialize the data and call the event
                string data = new StreamReader(request.EndGetResponse(result).GetResponseStream()).ReadToEnd();
                Data.ScoreResponse scoreResult = JsonConvert.DeserializeObject<Data.ScoreResult>(data).Response;

                if (scoreResult.Success[0] == 't')
                {
                    //
                    if (scoreResult.Scores == null)
                        _scores = new AScore[0];
                    else
                    {
                        //
                        List<Data.RawScore> scores = scoreResult.Scores;
                        AScore[] newScores = new AScore[scores.Count];

                        // Convert trophies into ATrophy
                        if (scores != null)
                        {
                            int length = scores.Count;
                            for (int i = 0; i < length; i++)
                                newScores[i] = new AScore(scores[i]);
                        }

                        // 
                        _scores = newScores;
                    }

                    //
                    if (OnFetchComplete != null)
                        OnFetchComplete(this);
                }
                else
                {
                    //
                    Data.SuccessResponse successResult = JsonConvert.DeserializeObject<Data.SuccessResult>(data).Response;

                    //
                    if (OnFetchError != null)
                        OnFetchError(this, successResult.Message);
                }
            }
        }
    }
}
