namespace Fanex.NAP.SPU.CTS.Shares
{
    using Fanex.RemotingRef;

    public static class CachedRef
    {
        private static readonly RemotingRefService refService = new RemotingRefService();

        public static string GetTeamName(int teamId, string lang = "en") => refService.GetTeamName(teamId.ToString(), lang);

        public static string GetSportTypeName(int sportId, string lang = "en") => refService.GetSportName(sportId.ToString(), lang);

        public static string GetBettypeName(int bettypeId, string lang = "en") => refService.GetBetTypeName(bettypeId.ToString(), lang);

        public static string GetLeagueName(int leagueId, string lang = "en") => refService.GetLeagueName(leagueId.ToString(), lang);

        public static string GetOddsTypeName(int oddsId, string lang = "en") => refService.GetOddsTypeName(oddsId.ToString(), lang);

        public static string GetOddsTypeName(string oddsType, string lang = "en") => refService.GetOddsTypeName(oddsType, lang);
    }
}
