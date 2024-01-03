using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using CsvHelper;

namespace Audacia.OrToolsPlayground.Examples.PickFplTeam.Models
{
    public class PickFplTeamModel
    {
        private PickFplTeamModel(List<FplPlayer> players, PickFplTeamOptions options)
        {
            Players = players;
            Options = options;
        }

        public static PickFplTeamModel FromCsv(PickFplTeamOptions options)
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "Audacia.OrToolsPlayground.Examples.PickFplTeam.Data.players_raw.csv";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream!);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var players = csv.GetRecords<FplPlayer>()
                .Where(p => p.ShouldAdd)
                .ToList();
            return new PickFplTeamModel(players, options);
        }
        
        public List<FplPlayer> Players { get; }

        public PickFplTeamOptions Options { get; }
    }
}