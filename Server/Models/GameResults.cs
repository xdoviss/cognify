using System;
using System.Collections;
using System.Collections.Generic;
using cognify.Shared;

namespace cognify.Server.Models
{
    public class GameResults : IEnumerable<GameResult>
    {
        private List<GameResult> results = new List<GameResult>();

        public void AddResult(GameResult result)
        {
            results.Add(result);
        }

        public IEnumerator<GameResult> GetEnumerator()
        {
            return results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }                                                                           
    }
}