// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/athletes/athletes.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var self = this;
            this.el = $(element);
            ESPN.init(ESPNAPIGlobals.apiKey);
            ESPN.getSports()
                .done(function (data) {
                    self.processSports(data);

                })
                .error(function (data) {
                    console.log("error");
                    console.log(data.statusText);
                })
            // TODO: Initialize the page here.

            this.el.find('#sport-select').change(function (data) {
                var value = $(this).val();
                self.el.find('.teams-button').show();
                self.getLeaguesBySport(value);
            })

            this.el.find('.teams-button').hide();
            this.el.find('.teams-button').click(function (data) {
                var text = self.getLabels();
                self.getTeamsByLeague(text);
            })
            this.el.find('#leagues-select').change(function (data) {
                var text = self.getLabels();
                self.getTeamsByLeague(text);
            })

            this.el.find('.popular-button').click(function (data) {

                var text = self.getLabels();
                self.getAllAthletesByLeague(text);
            })

        },

        getLabels: function () {
            var value = this.el.find('#sport-select').val();
            var secondValue = this.el.find('#leagues-select').val();

            var text = value;
            var team = this.el.find('#team-select').val();
            if (secondValue && secondValue != undefined)
             {
                text += "/" + secondValue;
        }

            return (text);

},

getAllAthletesByLeague: function (league, team) {
    var self = this;
    ESPN.getAllAthletesByLeague(league)
        .done(function (data) {
            self.processAthletes(data);

        })
        .error(function (err) {
            console.log("error" + err);
        })

},

processAthletes: function (data) {

    var self = this;
    this.el.find('.ath-resultsList').empty();
    var leagues = data.sports[0].leagues;
    var len = leagues.length;

    var dataArray = [];
    for (var i = 0; i < len; i++) {
        var item = leagues[i];
        var categoryName = item.name;
        var athletes = item.athletes;
        if (athletes) {
            var llen = athletes.length;
            for (var j = 0; j < llen; j++) {
                var id = athletes[j].id;

                var label = athletes[j].displayName;


                this.el.find('.ath-resultsList').append(window.toStaticHTML("<li><a href='" + id + "'>" + label + "</li>"));

            }
        }
    }

    this.el.find('.ath-resultsList a').click(function (evt) {
        evt.preventDefault();
        var id = $(this).attr('href');
        var value = self.el.find('#sport-select').val();
        var secondValue = self.el.find('#leagues-select').val();

        var text = value;
        var team = self.el.find('#team-select').val();
        if (secondValue && secondValue != undefined) {

            text += "/" + secondValue;
        }
        self.getDataForAthleteByTeamAndId(text, id)
    })
},

getDataForAthleteByTeamAndId: function (league, id) {

    this.el.find('#team-select').show();
    var self = this;
    ESPN.getDataForAthleteByLeagueAndId(league, id)
        .done(function (data) {
          
            WinJS.Navigation.navigate('pages/athletes/detail/athletedetail.html', data);
        })
        .error(function (err) {
            console.log("error" + err);
        })
},

getTeamsByLeague: function (league) {
   
    this.el.find('#team-select').show();
    var self = this;
    ESPN.getAllTeamsByLeague(league)
        .done(function (data) {
            self.processTeams(data);
        })
        .error(function (err) {
            console.log("error" + err);
        })
},

processTeams: function (data) {
    this.el.find('#team-select').empty();

    var leagues = data.sports[0].leagues;
    var len = leagues.length;
    var dataArray = [];
    for (var i = 0; i < len; i++) {
        var item = leagues[i];
        var categoryName = item.name;

        var teams = item.teams;

        if (teams) {
            var llen = teams.length;
            for (var j = 0; j < llen; j++) {
                var id = teams[j].id;
                var label = teams[j].location + " " + teams[j].name;
                this.el.find('#team-select').append(window.toStaticHTML("<option value='" + id + "'>" + label + "</option>"));

            }
        }
    }



},

processSports: function (data) {
    var sports = data.sports;
    var len = sports.length;
    var dataArray = [];
    var id = '';
    for (var i = 0; i < len; i++) {
        var item = sports[i];
        var categoryName = item.name;
        
        if (i == 0) {
            id = categoryName;
            this.el.find('#sport-select').append(window.toStaticHTML("<option selected value='" + categoryName + "'>" + categoryName + "</option>"));

        } else {
            this.el.find('#sport-select').append(window.toStaticHTML("<option value='" + categoryName + "'>" + categoryName + "</option>"));

        }

    }
    this.getLeaguesBySport(this.el.find('#sport-select').val());
},
getLeaguesBySport: function (sport) {
    var self = this;
    ESPN.getLeaguesBySport(sport, {})
        .done(function (data) {
            self.processLeagues(data);

        })
        .error(function (data) {
            console.log("error");
            console.log(data.statusText);
        })
    // TODO: Initialize the
},
getNowTop: function () {
    var self = this;
    ESPN.getNowTop()
        .done(function (data) {
            self.processResult(data);

        })
        .error(function (data) {
            console.log("error");
            console.log(data.statusText);
        })
},


processLeagues: function (data) {

    this.el.find('#leagues-select').empty();
    var sports = data.sports;
    var len = sports.length;
    var dataArray = [];
    for (var i = 0; i < len; i++) {
        var item = sports[i];
        var categoryName = item.name;

        var leagues = item.leagues;
        if (leagues) {
            var llen = leagues.length;
            for (var j = 0; j < llen; j++) {

                var label = leagues[j].abbreviation;

                dataArray.push({
                    name: label
                });
                this.el.find('#leagues-select').append(window.toStaticHTML("<option value='" + label + "'>" + label + "</option>"));
            }
        }
    }

},


unload: function () {
    // TODO: Respond to navigations away from this page.
},

updateLayout: function (element, viewState, lastViewState) {
    /// <param name="element" domElement="true" />

    // TODO: Respond to changes in viewState.
}
});
})();