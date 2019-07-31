using System;
using System.Linq;

namespace TestSqlite
{
    internal class Player
    {
        public string Id { get; set; }
        public string TraceId { get; set; }
        public DateTime CreatedOn { get; set; }

        public Player(string id, string traceId, DateTime createdOn)
        {
            Id = id;
            TraceId = traceId;
            CreatedOn = createdOn;
        }

        public static Player[] TestData = new Player[]
        {
            new Player("1", Guid.NewGuid().ToString(), DateTime.Now),
            new Player("2", Guid.NewGuid().ToString(), DateTime.Now),
        };
    }
}