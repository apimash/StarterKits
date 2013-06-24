// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";
    var publicMembers = {
        sportList: new WinJS.Binding.List(),
        locations: []
    };
    WinJS.Namespace.define("ESPNTeams", publicMembers);
    WinJS.UI.Pages.define("/pages/teams/teams.html", {
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
                console.log(value);
                self.el.find('.teams-button').show();
                self.getLeaguesBySport(value);
            })
            this.el.find('.teams-button').hide();
            this.el.find('.teams-button').click(function (data) {
                var value = $('#sport-select').val();
                var secondValue = $('#leagues-select').val();

                var text = value;
                if (secondValue && secondValue != undefined) {
                    text += "/" + secondValue;
                }
                console.log(text);
                self.getTeamsByLeague(text);
            })

        },

        processSports: function (data) {
            var sports = data.sports;
            var len = sports.length;
            var dataArray = [];
            for (var i = 0; i < len; i++) {
                var item = sports[i];
                var categoryName = item.name;
                console.log(categoryName);
                this.el.find('#sport-select').append(window.toStaticHTML("<option value='" + categoryName + "'>" + categoryName + "</option>"));

            }

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

                        console.log(label);
                        dataArray.push({
                            name: label
                        });
                        this.el.find('#leagues-select').append(window.toStaticHTML("<option value='" + label + "'>" + label + "</option>"));
                    }
                }


            }

        },


        getTeamsByLeague: function (league) {
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
            this.el.find('.team-list').empty();
            var leagues = data.sports[0].leagues;
            var len = leagues.length;
            var dataArray = [];
            for (var i = 0; i < len; i++) {
                var item = leagues[i];
                var categoryName = item.name;

                var teams = item.teams;
                console.log(teams.length);
                if (teams) {
                    var llen = teams.length;
                    for (var j = 0; j < llen; j++) {
                        var label = teams[j].location + " " + teams[j].name;
                        this.el.find('.team-list').append(window.toStaticHTML("<li>" + label + "</li>"));

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