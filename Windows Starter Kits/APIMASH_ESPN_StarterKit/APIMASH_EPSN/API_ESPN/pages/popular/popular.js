// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/popular/popular.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            // TODO: Initialize the page here.
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
            var self = this;
            this.el.find('.league-container').hide();
            this.el.find('.popular-button').hide();
            this.el.find('#team-select').hide();
            this.el.find('.api-select').change(function (evt) {
                evt.preventDefault();
                self.processSelection();
            });

            this.el.find('#sport-select').change(function (data) {
                var value = $(this).val();
                console.log(value);
                self.el.find('.popular-button').show();
                self.getLeaguesBySport(value);
            })

            this.el.find('.popular-button').click(function (data) {

                var value = self.el.find('#sport-select').val();
                var text = value;
                var secondValue = self.el.find('#leagues-select').val();


                if (secondValue && secondValue != undefined) {
                    console.log(secondValue);
                    text += "/" + secondValue;
                }

                if ($(this).text() == "Get News") {
                    var team = self.el.find('#team-select').val();
                    self.getNow([text], [team]);
                } else {
                    self.getTeamsByLeague(text);
                }


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
            this.el.find('.popular-button').text("Get News");
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

        processSelection: function () {
            var val = $('.api-select').val();
            console.log(this);
            switch (val) {
                case "1":
                    this.el.find('.league-container').show();
                    this.el.find('.resultsList').empty();
                    break;
                case "2":
                    this.getNowTop();
                    break;
                case "3":
                    this.getNowPopular();
                    break;
                case "4":
                    this.getTopHomeHeadlines();
                    break;

            }
        },
        prepNow: function () {
            this.el.find('.league-container').show();
        },
        processSports: function (data) {
            var sports = data.sports;
            var len = sports.length;
            var dataArray = [];
            for (var i = 0; i < len; i++) {
                var item = sports[i];
                var categoryName = item.name;
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
        processResult: function (data) {

            this.el.find('.resultsList').empty();
            var feed = data.feed;
            if (!feed) {
                feed = data.headlines;
            }
            var len = feed.length;
            var dataArray = [];
            for (var i = 0; i < len; i++) {
                var item = feed[i];
                var headline = item.headline;
                var url = item.links.web;
                this.el.find('.resultsList').append(window.toStaticHTML("<li><a href='" + url + "'>" + headline + "</li>"));

            }
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

        getTopNewsByLeague: function (league) {
            var self = this;
            ESPN.getTopNewsByLeague(league)
                .done(function (data) {
                    self.processResult(data);

                })
                .error(function (data) {
                    console.log("error");
                    console.log(data.statusText);
                })
        },
        getTopHomeHeadlines: function () {
            var self = this;
            ESPN.getTopHomeHeadlines()
                .done(function (data) {
                    self.processResult(data);

                })
                .error(function (data) {
                    console.log("error");
                    console.log(data.statusText);
                })
        },
        getNowPopular: function () {
            var self = this;
            ESPN.getNowPopular()
                .done(function (data) {
                    self.processResult(data);

                })
                .error(function (data) {
                    console.log("error");
                    console.log(data.statusText);
                })
        },

        getNow: function (leagues, teams) {
            var self = this;
            ESPN.getNow(leagues, teams)
                .done(function (data) {
                    self.processResult(data);

                })
                .error(function (data) {
                    console.log("error");
                    console.log(data.statusText);
                })
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