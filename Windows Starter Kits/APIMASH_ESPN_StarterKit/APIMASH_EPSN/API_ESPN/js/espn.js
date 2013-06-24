
var ESPN = function () {
    /*
    DEPENDANCIES: jquery for the JSON calls & promises
    IMPORTANT:Please register with Mashery and ESPN to get an api key (http://developer.espn.com/)
    NOTE: Didn't want to use require for this, but it would be easy to port it
    INITIALIZE: You need to init this class with your apikey ( ESPN.init('your-app-key-here');)
    READ: Read the espn documentation to see what type of paramters you can pass in - didn't duplicate documentation here. 
    All calls are documented and show example usage. 
    Use at own risk!
    Stacey Mulcahy | @bitchwhocodes
    */
    LEAGUE_MLB = 'baseball/mlb';
    LEAGUE_NBA = 'basketball/nba';
    LEAGUE_WNBA = 'basketball/wnba';
    LEAGUE_NHL = 'hockey/nhl';
    LEAGUE_NFL = 'football/nfl';
    LEAGUE_NCAA_BBALL_MENS = "basketball/mens-college-basketball";
    LEAGUE_NCAA_BBALL_WOMENS = "basketball/womens-college-basketball";
    LEAGUE_NCAA_FOOTBALL = 'football/college-football';
    LEAGUE_NCAA_HOCKEY_MEN = 'hockey/mens-college-hockey';
    LEAGUE_NCAA_HOCKEY_WOMEN = 'hockey/mens-college-hockey';
    LEAGUE_GOLF_PGA = "golf/pga";
    LEAGUE_TENNIS_PRO = "tennis/atp";
    LEAGUE_TENNIS_WOMENS = "tennis/wta";
    LEAGUE_BOXING = "boxing";
    LEAGUE_FIGHTING_UFC = "mna/ufc";
    LEAGUE_HORSERACING = "horseracing";
    LEAGUE_OLYMPICS = "olympics";
    LEAGUE_NCAA_VOLLEYBALL_MENS = "volleyball/mens-college-volleyball";
    LEAGUE_NCAA_VOLLEYBALL_WOMENS = "volleyball/womens-college-volleyball";
    LEAGUE_NCAA_LACROSSE_MENS = "lacrosse/mens-college-lacrosse";
    LEAGUE_NCAA_LACROSSE_WOMENS = "lacrosse/womens-college-lacrosse";
    LEAGUE_NCAA_WATERPOLO_MENS = "water-polo/mens-college-water-polo";
    LEAGUE_NCAA_WATERPOLO_WOMENS = "water-polo/womens-college-water-polo";

    /* More odd racing leagues, check them out via utility functions */

    LEAGUE_RACING_FORMULA = "racing/f1";
    LEAGUE_RACING_NASCAR = "racing/nationwide";
    /*
    There are many other leagues. Soccer lists a gazillion of them. You can use the utility function of getSports to see them all. 
    */
    var init = function (mykey) {

        apikey = mykey;// Required
    }

    var endPoints = {
        'now_getNow': 'http://api.espn.com/v1/now',
        'now_getNowTop': 'http://api.espn.com/v1/now/top',
        'now_getNowPopular': 'http://api.espn.com/v1/now/popular',
        'news_getTopHomeHeadlines': 'http://api.espn.com/v1/sports/news/headlines/top',
        'news_getTopNews': 'http://api.espn.com/v1/sports/news/headlines',
        'news_getTopNewsByLeague': 'http://api.espn.com/v1/sports/{sport/league}/news/headlines',
        'news_getNewsByLeague': 'http://api.espn.com/v1/sports/{sport/league}/news',
        'news_getFantastyFootball': 'http://api.espn.com/v1/fantasy/football/news',
        'news_getFullTextForStoryById': 'http://api.espn.com/v1/sports/news/{id}',
        'athletes_getAllAthletesByLeague': 'http://api.espn.com/v1/sports/{sport/league}/athletes',
    
        'athletes_getDataForAthleteLeagueAndId': 'http://api.espn.com/v1/sports/{sport/league}/athletes/{id}',
        'teams_getAllTeamsByLeague': 'http://api.espn.com/v1/sports/{sport/league}/teams',
        'teams_getTeamByLeagueAndId': 'http://api.espn.com/v1/sports/{sport/league}/teams/{id}',
        'utility_getSports': 'http://api.espn.com/v1/sports',
        'utility_getLeaguesBySport': 'http://api.espn.com/v1/sports/{sport}',
        'utility_getInformationByLeague': 'http://api.espn.com/v1/sports/{sport/league}'

    }

    /*
    Stream of the latest, most-recently published content on ESPN.com.
    Parameters:None
    Example:    ESPN.getNow().done(doneHandler).error(errorHandler);
    */
    var getNow2 = function () {
        return (constructBasicJsonCall('now_getNow'));
    }
    /*
    Top editorially-curated content on ESPN.com.
    Parameters:None
    Example:    ESPN.getNowTop().done(doneHandler).error(errorHandler);
    */
    var getNowTop = function () {
        return (constructBasicJsonCall('now_getNowTop'));
    }

    /*
    Most recent content for a particular league, can identify team
    Returns: Promise Object
    Parameters: leagues (Array:Optional), teams(Array:Optional), options(Object:Optional );
    Example:    ESPN.getNow([ESPN.LEAGUE_NHL,ESPN.LEAGUE_NBA]).done(doneHandler).error(errorHandler);
    */
    var getNow = function (leagues, teams, options) {
        var url = endPoints['now_getNow'];
        if (!options) {
            options = {};
        }
        options = formatOptions(options, leagues, teams);
        return (makeJSONRequest(url, options));
    }

    /*
    Most popular, recently published content on ESPN.com.
    Parameters:None
    Example:    ESPN.getNowPopular().done(doneHandler).error(errorHandler);
    */
    var getNowPopular = function () {
        return (constructBasicJsonCall('now_getNowPopular'));
    }

    /*
    Top editorially-curated news as shown on ESPN.com home page.
    Parameters:None
    Example:    ESPN.getTopHomeHeadlines().done(doneHandler).error(errorHandler);
    */
    var getTopHomeHeadlines = function () {
        return (constructBasicJsonCall('news_getTopHomeHeadlines'));
    }
    /*
    Top editorially-curated news aggregated across all sports.
    Parameters:None
    Example:    ESPN.getTopNews().done(doneHandler).error(errorHandler);
    */
    var getTopNews = function () {
        return (constructBasicJsonCall('news_getTopNews'));
    }

    /*
    Top editorially-curated news by league
    Parameters: league(String:Required),options(Object:Optional)
    Example:    ESPN.getTopNewsByLeague(EPSN.LEAGUE_NFL).done(doneHandler).error(errorHandler)
    */
    var getTopNewsByLeague = function (league, options) {
        var url = endPoints['news_getTopNewsByLeague'];
        var newUrl = url.replace('{sport/league}', league);
        return (makeJSONRequest(newUrl));
    }

    /*
     League news published on a specific date.
     Parameters:league(String:Required),options(Object:Optional)
     Example:    ESPN.getNewsByLeague(EPSN.LEAGUE_NFL).done(doneHandler).error(errorHandler)
    */
    var getNewsByLeague = function (league, options) {
        var url = endPoints['news_getNewsByLeague'];
        var newUrl = url.replace('{sport/league}', league);
        if (!options) {
            options = {};
        }
        return (makeJSONRequest(newUrl, options));
    }

    /*
    Get latest fantasy football
    Current day's Fantasy Football news from ESPN's experts.
    Parameters:options(Object, optional);
    Example:    ESPN.getFantasyFootball().done(doneHandler).error(errorHandler);
    */
    var getFantasyFootball = function (options) {

        return (constructBasicJsonCall('news_getFantastyFootball', options));

    }

    /*
    Get the full text for a story by its id
    Parameters: id(String,Required), options(Object:optional)
    Example:    ESPN.getFullTextForStoryById('45673').done(doneHandler).error(errorHandler)

    */
    var getFullTextForStoryById = function (id, options) {
        var url = endPoints['news_getFullTextForStoryById'];
        var newUrl = url.replace('{id}', id);
        if (!options) {
            options = {};
        }
        return (makeJSONRequest(newUrl));
    }

    /*
    Get All athletes By league
    Parameters: league(String:Required),team:(String,optiona;).options(Object,optional)
    Example:    ESPN.getAllAthletesByLeague(ESPN.LEAGUE_NFL).done(doneHandler).error(errorHandler);
    */
    var getAllAthletesByLeague = function (league, options) {

        var url = endPoints['athletes_getAllAthletesByLeague'];
        var newUrl = url.replace('{sport/league}', league);
       
        if (!options) {
            options = {};
        }
        return (makeJSONRequest(newUrl, options));
    }

    /*
    Get data for a specific athlete
    Parameters: league(String:Required), id(String:Required),options(Object,optional)
    Example:    ESPN.getDataForAthleteByLeagueAndId(ESPN.LEAGUE_NHL,'6651').done(doneHandler).error(errorHandler)
    */
    var getDataForAthleteByLeagueAndId = function (league, id, options) {
        var url = endPoints['athletes_getDataForAthleteLeagueAndId'];
        var newUrl = url.replace('{sport/league}', league);
        newUrl = newUrl.replace('{id}', id);
        console.log(newUrl);
        return (makeJSONRequest(newUrl, options));
    }

    /*
    Get a listing of all the teams by a league
    Parameters:league(String:Required), options(Object:Optional);
    Example:    ESPN.getAllTeamsByLeague(ESPN.LEAGUE_NHL).done(doneHandler).error(errorHandler)
    */
    var getAllTeamsByLeague = function (league, options) {
        var url = endPoints['teams_getAllTeamsByLeague'];
        var newUrl = url.replace('{sport/league}', league);

        if (!options) {
            options = {};
        }
        return (makeJSONRequest(newUrl, options));
    }

    /*
    Get team information by League and team id
    Parameters:league(String:Required), id(String:Required), options(Object,optional)
    Example:    EPSN.getTeamByLeagueAndId(EPSN.LEAGUE_NBA,'1').done(doneHandler).error(errorHandler);
    */
    var getTeamByLeagueAndId = function (league, id, options) {
        var url = endPoints['teams_getTeamByLeagueAndId'];
        var newUrl = url.replace('{sport/league}', league);
        newUrl = newUrl.replace('{id}', id);
        if (!options) {
            options = {};
        }

        return (makeJSONRequest(newUrl, options));
    }

    /*
    Get all sports supported or included in the api
    Parameters:None
    Example:    EPSN.getSports().done(doneHandler).error(errorHandler);
    */
    var getSports = function () {
        return (constructBasicJsonCall('utility_getSports'));
    }

    /*
   Get all leagues by the sport
   Parameters:sport(String:Required), options(Object:optional);
   Example: ESPN.getLeaguesBySport('basketball').done(doneHandler).error(errorHandler);
   */
    var getLeaguesBySport = function (sport, options) {
        var url = endPoints['utility_getLeaguesBySport'];

        var newUrl = url.replace('{sport}', sport);
        if (!options) {
            options = {};
        }
        return (makeJSONRequest(newUrl, options));
    }

    /*
    Get information for a league
    Parameters:league(String:Required), options(Object:Optional)
    */
    var getInformationByLeague = function (league, options) {

        var url = endPoints['utility_getInformationByLeague'];
        var newUrl = url.replace('{sport/league}', league);
        if (!options) {
            options = {};
        }

        return (makeJSONRequest(newUrl, options));
    }

    /*
    private
    */
    var makeJSONRequest = function (url, data) {
        var self = this;
        if (!data) { data = {} }
        data['apikey'] = apikey;
        console.log(url);
        var jqxhr = $.getJSON(url, data);
        return (jqxhr);

    }
    /*
    private
    */
    var constructBasicJsonCall = function (endpoint, options) {
        var url = endPoints[endpoint];

        if (!options) {
            options = {};
        }
        return (makeJSONRequest(url, options));
    }
    /*
    Private Utility function
    */
    var formatOptions = function (options, leagues, teams) {
        if (leagues) {
            var len = leagues.length;
            for (var i = 0; i < len; i++) {
                leagues[i] = leagues[i].split('/')[1];
            }
            options.leagues = leagues.join(',');
        }

        if (teams) {
            options.teams = teams.join(',');
        }

        return (options);
    }

    return {
        // public properties
        LEAGUE_NBA: LEAGUE_NBA,
        LEAGUE_MLB: LEAGUE_MLB,
        LEAGUE_NFL: LEAGUE_NFL,
        LEAGUE_NHL: LEAGUE_NHL,
        init: init,
        getNow: getNow,
        getNowTop: getNowTop,
        getNowPopular: getNowPopular,
        getTopHomeHeadlines: getTopHomeHeadlines,
        getTopNews: getTopNews,
        getTopNewsByLeague: getTopNewsByLeague,
        getNewsByLeague: getNewsByLeague,
        getFantasyFootball: getFantasyFootball,
        getFullTextForStoryById: getFullTextForStoryById,
        getAllAthletesByLeague: getAllAthletesByLeague,
        getDataForAthleteByLeagueAndId: getDataForAthleteByLeagueAndId,
        getAllTeamsByLeague: getAllTeamsByLeague,
        getTeamByLeagueAndId: getTeamByLeagueAndId,
        getSports: getSports,
        getLeaguesBySport: getLeaguesBySport,
        getInformationByLeague: getInformationByLeague

    }
}();