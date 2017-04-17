# APIMASH StackExchange Starter Kit
## Date: 5.10.2013
## Version: v1.0.0
## Author(s): Michael Cummings
## URL: http://github.com/apimash/starterkits

----------
## Description
The StackExchange Starter Kit is a XAML/C# Windows 8 app based on the Blank Template that demonstrates calling the StackExchange REST API's. The JSON payload for Questions, Answers, Comments, Users is deserialized into a set of C# classes that define the Data Model. That data then is selectively copied into the View Model for binding to WinRT XAML controls. You can use the breadth and detail of the discussion information available through the StackExchange API to create mashups, visualizations and other applications that will provide an added dimension of user experience for the answer seeker.

![alt text][1001]

## Features
 - Invokes the [StackExchange REST API][1002]
 - Demonstrates how to deserialize compressed JSON to C# and bind to WinRT XAML Controls
 - Provides a baseline for a Windows 8 Store App

## Requirements

 - Windows 8
 - Visual Studio 2012 Express for Windows 8 or higher
 - [JSON.NET from Newtonsoft][1003]
 - [HTML Agility Pack][1005]

## Setup

 - [Download the Starter Kit Zip Portfolio][1004] 
 - Open the Solution in Visual Studio
 - Compile and Run

## Customization

The StackExchange API, one a scale of 1 to 10, where 1 is simple and 10 is complex, is an 9 :). StackExchange provides a rich set of API collections each with several API's and methods that give you access to Questions, Answers, Comments, Users, and Tags that can be used together or with other API's to create compelling apps.

StackExchange provides these 4 API collections:

 - Per-Site API's
 - Network API's

The StackExchange Starter Kit uses the Question API's:

The Starter Kit starts by calling the ***questions*** method of the API to retrieve a list of questions. [*See Line 81 in the *MainPage.Xaml.cs* file*].

You can customize the initial call by modifying line 81 in *MainPage.Xaml.cs*.

To experiment further you can look at the additional capabilities of the Stack Exchange API's.

## Stack Exchange API v2.0

This is the documentation for the v2.0 **read-only** (with optional [authentication][1]) Stack Exchange API. If you have additional questions, or believe you have encountered a bug, don't hesitate to post a question on [Stack Apps][2].

If your application is in a runnable state (even a beta one), Stack Apps is also the place to [list it][3].

## General

Applications should be registered on [Stack Apps][4] to get a request key. Request keys grant more requests per day, and are necessary for using access_tokens created via [authentication][1].

All API responses are [JSON][5], we do support [JSONP][6] with the `callback` query parameter. Every response in the API is returned in a common ["wrapper" object][7], for easier and more consistent parsing.

Additionally, all API responses are compressed. The `Content-Encoding` header is always set, but some proxies will strip this out. The proper way to decode API responses can be found [here][8].

API usage is subject to a [number of throttles][9]. In general, applications should strive to make as few requests as possible to satisfy their function.

API responses are heavily cached. Polling for changes should be done sparingly in any case, and polling at a rate faster than once a minute (for semantically identical requests) is considered abusive.

Developers can trim API responses down to just the fields they are interested in using [custom filters][10]. Many types have fields that are not normally returned (question bodies, for example) that can likewise be requested via a custom filter.

There are a few methods which require that the application be acting on behalf of a user in order to be invoked. For authentication purposes, the Stack Exchange API [implements OAuth 2.0][1] (templated on Facebook's implementation in pursuit of developer familiarity).

A number of methods in the Stack Exchange API accept dates as parameters and return dates as properties, the format of these dates is [consistent and documented][11]. The cliff-notes version is, all dates in the Stack Exchange API are in [unix epoch time][12].

Unless otherwise noted, the maximum size of any page is `100`, any `{ids}` parameter likewise is capped at `100` elements, all indexes start at 1.

If a parameter name is plural it accepts [vectorized requests][13], otherwise a single value may be passed. Compare [users/{id}/inbox][14] and [/users/{ids}][15].

It is possible to compose [reasonably complex queries][16] against the live Stack Exchange sites using the `min`, `max`, `fromdate`, `todate`, and `sort` parameters. Most, but not all, methods accept some or all of these parameters, the documentation for individual methods will highlight which do. Most methods also have a common set of [paging parameters][17].


## Per-Site Methods

Each of these methods operates on a single site at a time, identified by the `site` parameter. This parameter can be the full domain name (ie. "stackoverflow.com"), or a short form identified by `api_site_parameter` on the [site object][20].

### Answers

[/answers][21] Get all answers on the site. 

[/answers/{ids}][22] Get answers identified by a set of ids. 

[/answers/{ids}/comments][23] Get comments on the answers identified by a set of ids. 

### Badges

[/badges][24] Get all badges on the site, in alphabetical order. 

[/badges/{ids}][25] Get the badges identified by ids. 

[/badges/name][26] Get all non-tagged-based badges in alphabetical order. 

[/badges/recipients][27] Get badges recently awarded on the site. 

[/badges/{ids}/recipients][28] Get the recent recipients of the given badges. 

[/badges/tags][29] Get all tagged-based badges in alphabetical order. 

### Comments

[/comments][30] Get all comments on the site. 

[/comments/{ids}][31] Get comments identified by a set of ids. 

[/comments/{id}/delete][32] Delete a comment identified by its id. auth required 

[/comments/{id}/edit][33] Edit a comment identified by its id. auth required 

### Events

[/events][34] Get recent events that have occurred on the site. Effectively a stream of new users and content. auth required 

### Info

[/info][35] Get information about the entire site. 

### Posts

[/posts][36] Get all posts (questions and answers) in the system. 

[/posts/{ids}][37] Get all posts identified by a set of ids. Useful for when the type of post (question or answer) is not known. 

[/posts/{ids}/comments][38] Get comments on the posts (question or answer) identified by a set of ids. 

[/posts/{id}/comments/add][39] Create a new comment on the post identified by id. auth required 

[/posts/{ids}/revisions][40] Get revisions on the set of posts in ids. 

[/posts/{ids}/suggested-edits][41] Get suggested edits on the set of posts in ids. 

### Privileges

[/privileges][42] Get all the privileges available on the site. 

### Questions

[/questions][43] Get all questions on the site. 

[/questions/{ids}][44] Get the questions identified by a set of ids. 

[/questions/{ids}/answers][45] Get the answers to the questions identified by a set of ids. 

[/questions/{ids}/comments][46] Get the comments on the questions identified by a set of ids. 

[/questions/{ids}/linked][47] Get the questions that link to the questions identified by a set of ids. 

[/questions/{ids}/related][48] Get the questions that are related to the questions identified by a set of ids. 

[/questions/{ids}/timeline][49] Get the timelines of the questions identified by a set of ids. 

[/questions/featured][50] Get all questions on the site with active bounties. 

[/questions/unanswered][51] Get all questions the site considers unanswered. 

[/questions/no-answers][52] Get all questions on the site with **no** answers. 

### Revisions

[/revisions/{ids}][53] Get all revisions identified by a set of ids. 

### Search

[/search][54] Search the site for questions meeting certain criteria. 

[/search/advanced][55] Search the site for questions using most of the on-site search options. 

[/similar][56] Search the site based on similarity to a title. 

### Suggested Edits

[/suggested-edits][57] Get all the suggested edits on the site. 

[/suggested-edits/{ids}][58] Get the suggested edits identified by a set of ids. 

### Tags

[/tags][59] Get the tags on the site. 

[/tags/{tags}/info][60] Get tags on the site by their names. 

[/tags/moderator-only][61] Get the tags on the site that only moderators can use. 

[/tags/required][62] Get the tags on the site that fulfill required tag constraints. 

[/tags/synonyms][63] Get all the tag synonyms on the site. 

[/tags/{tags}/faq][64] Get frequently asked questions in a set of tags. 

[/tags/{tags}/related][65] Get related tags, based on common tag pairings. 

[/tags/{tags}/synonyms][66] Get the synonyms for a specific set of tags. 

[/tags/{tag}/top-answerers/{period}][67] Get the top answer posters in a specific tag, either in the last month or for all time. 

[/tags/{tag}/top-askers/{period}][68] Get the top question askers in a specific tag, either in the last month or for all time. 

[/tags/{tags}/wikis][69] Get the wiki entries for a set of tags. 

### Users

All user methods that take an `{ids}` parameter have a `/me` equivalent method that takes an `access_token` instead. These methods are provided for developer convenience, with the exception of plain [/me][70], which is actually necessary for discovering which user [authenticated][1] to an application.

[/users][71] Get all users on the site. 

[/users/{ids}][15] [/me][72] Get the users identified by a set of ids. 

[/users/{ids}/answers][73] [/me/answers][74] Get the answers posted by the users identified by a set of ids. 

[/users/{ids}/badges][75] [/me/badges][76] Get the badges earned by the users identified by a set of ids. 

[/users/{ids}/comments][77] [/me/comments][78] Get the comments posted by the users identified by a set of ids. 

[/users/{ids}/comments/{toid}][79] [/me/comments/{toid}][80] Get the comments posted by a set of users in reply to another user. 

[/users/{ids}/favorites][81] [/me/favorites][82] Get the questions favorited by users identified by a set of ids. 

[/users/{ids}/mentioned][83] [/me/mentioned][84] Get the comments that mention one of the users identified by a set of ids. 

[/users/{ids}/merges][85] [/me/merges][86] Get the merges a user's accounts has undergone. 

[/users/{id}/notifications][87] [/me/notifications][88] Get a user's notifications. 

[/users/{id}/notifications/unread][89] [/me/notifications/unread][90] Get a user's unread notifications. 

[/users/{id}/privileges][91] [/me/privileges][92] Get the privileges the given user has on the site. 

[/users/{ids}/questions][93] [/me/questions][94] Get the questions asked by the users identified by a set of ids. 

[/users/{ids}/questions/featured][95] [/me/questions/featured][96] Get the questions on which a set of users, have active bounties. 

[/users/{ids}/questions/no-answers][97] [/me/questions/no-answers][98] Get the questions asked by a set of users, which have **no** answers. 

[/users/{ids}/questions/unaccepted][99] [/me/questions/unaccepted][100] Get the questions asked by a set of users, which have at least one answer but no accepted answer. 

[/users/{ids}/questions/unanswered][101] [/me/questions/unanswered][102] Get the questions asked by a set of users, which are not considered to be adequately answered. 

[/users/{ids}/reputation][103] [/me/reputation][104] Get a subset of the reputation changes experienced by the users identified by a set of ids. 

[/users/{ids}/reputation-history][105] [/me/reputation-history][106] Get a history of a user's reputation, excluding private events. 

[/users/{ids}/reputation-history/full][107] [/me/reputation-history/full][108] Get a full history of a user's reputation. auth required 

[/users/{ids}/suggested-edits][109] [/me/suggested-edits][110] Get the suggested edits provided by users identified by a set of ids. 

[/users/{ids}/tags][111] [/me/tags][112] Get the tags that the users (identified by a set of ids) have been active in. 

[/users/{id}/tags/{tags}/top-answers][113] [/me/tags/{tags}/top-answers][114] Get the top answers a user has posted on questions with a set of tags. 

[/users/{id}/tags/{tags}/top-questions][115] [/me/tags/{tags}/top-questions][116] Get the top questions a user has posted with a set of tags. 

[/users/{ids}/timeline][117] [/me/timeline][118] Get a subset of the actions of that have been taken by the users identified by a set of ids. 

[/users/{id}/top-answer-tags][119] [/me/top-answer-tags][120] Get the top tags (by score) a single user has posted answers in. 

[/users/{id}/top-question-tags][121] [/me/top-question-tags][122] Get the top tags (by score) a single user has asked questions in. 

[/users/{ids}/write-permissions][123] [/me/write-permissions][124] Get the write access a user has via the API. 

[/users/moderators][125] Get the users who have moderation powers on the site. 

[/users/moderators/elected][126] Get the users who are active moderators who have also won a moderator election. 

[/users/{id}/inbox][14] [/me/inbox][127] Get a user's inbox. auth required 

[/users/{id}/inbox/unread][128] [/me/inbox/unread][129] Get the unread items in a user's inbox. auth required 

## Network Methods

These methods return data across the entire Stack Exchange network of sites. Accordingly, you do not pass a site parameter to them.

### Access Tokens

[/access-tokens/{accessTokens}/invalidate][130] Allows an application to dispose of `access_tokens` when it is done with them. 

[/access-tokens/{accessTokens}][131] Allows an application to inspect `access_tokens` it has, useful for debugging. 

### Applications

[/apps/{accessTokens}/de-authenticate][132] Allows an application to de-authorize itself for a set of users. 

### Errors

[/errors][133] Get descriptions of all the errors that the API could return. 

[/errors/{id}][134] Simulate an API error for testing purposes. 

### Filters

[/filters/create][135] Create a new [filter][10]. 

[/filters/{filters}][136] Decode a set of filters, useful for debugging purposes. 

### Inbox

[/inbox][137] Get a user's inbox, outside of the context of a site. auth required 

[/inbox/unread][138] Get the unread items in a user's inbox, outside of the context of a site. auth required 

### Notifications

[/notifications][139] Get a user's notifications, outside of the context of a site. auth required 

[/notifications/unread][140] Get a user's unread notifications, outside of the context of a site. auth required 

### Sites

[/sites][141] Get all the sites in the Stack Exchange network. 

### Users

[/users/{ids}/associated][142] [/me/associated][143] Get a user's associated accounts. 
 

*   [Authentication][1]
*   [Javascript SDK][144]
*   
*   [Batching Requests][13]
*   [Complex Queries][16]
*   [Filters][10]
*   [Paging][17]
*   
*   [Dates][11]
*   [Numbers][145]
*   
*   [Compression][8]
*   [Error Handling][146]
*   [Response Wrapper][7]
*   [Rate Limiting][9]
*   [Users][147]
*   
*   [Write][148]
*   
*   [Change Log][149]
*   
*   [Terms Of Use][150]

 [1]: /docs/authentication
 [2]: http://stackapps.com
 [3]: http://stackapps.com/questions/7/how-to-list-your-application-library-wrapper-here
 [4]: http://stackapps.com/apps/oauth/register
 [5]: http://en.wikipedia.org/wiki/JSON
 [6]: http://en.wikipedia.org/wiki/JSONP
 [7]: /docs/wrapper
 [8]: /docs/compression
 [9]: /docs/throttle
 [10]: /docs/filters
 [11]: /docs/dates
 [12]: http://en.wikipedia.org/wiki/Unix_time
 [13]: /docs/vectors
 [14]: /docs/user-inbox
 [15]: /docs/users-by-ids
 [16]: /docs/min-max
 [17]: /docs/paging
 [18]: /docs?tab=category#docs "group by concept"
 [19]: /docs?tab=type#docs "group by type"
 [20]: /docs/types/site
 [21]: /docs/answers
 [22]: /docs/answers-by-ids
 [23]: /docs/comments-on-answers
 [24]: /docs/badges
 [25]: /docs/badges-by-ids
 [26]: /docs/badges-by-name
 [27]: /docs/badge-recipients
 [28]: /docs/badge-recipients-by-ids
 [29]: /docs/badges-by-tag
 [30]: /docs/comments
 [31]: /docs/comments-by-ids
 [32]: /docs/delete-comment
 [33]: /docs/edit-comment
 [34]: /docs/events
 [35]: /docs/info
 [36]: /docs/posts
 [37]: /docs/posts-by-ids
 [38]: /docs/comments-on-posts
 [39]: /docs/create-comment
 [40]: /docs/revisions-by-ids
 [41]: /docs/posts-on-suggested-edits
 [42]: /docs/privileges
 [43]: /docs/questions
 [44]: /docs/questions-by-ids
 [45]: /docs/answers-on-questions
 [46]: /docs/comments-on-questions
 [47]: /docs/linked-questions
 [48]: /docs/related-questions
 [49]: /docs/questions-timeline
 [50]: /docs/featured-questions
 [51]: /docs/unanswered-questions
 [52]: /docs/no-answer-questions
 [53]: /docs/revisions-by-guids
 [54]: /docs/search
 [55]: /docs/advanced-search
 [56]: /docs/similar
 [57]: /docs/suggested-edits
 [58]: /docs/suggested-edits-by-ids
 [59]: /docs/tags
 [60]: /docs/tags-by-name
 [61]: /docs/moderator-only-tags
 [62]: /docs/required-tags
 [63]: /docs/tag-synonyms
 [64]: /docs/faqs-by-tags
 [65]: /docs/related-tags
 [66]: /docs/synonyms-by-tags
 [67]: /docs/top-answerers-on-tags
 [68]: /docs/top-askers-on-tags
 [69]: /docs/wikis-by-tags
 [70]: /docs/me
 [71]: /docs/users
 [72]: /docs/me "/me equivalent of /users/{ids}"
 [73]: /docs/answers-on-users
 [74]: /docs/me-answers "/me equivalent of /users/{ids}/answers"
 [75]: /docs/badges-on-users
 [76]: /docs/me-badges "/me equivalent of /users/{ids}/badges"
 [77]: /docs/comments-on-users
 [78]: /docs/me-comments "/me equivalent of /users/{ids}/comments"
 [79]: /docs/comments-by-users-to-user
 [80]: /docs/me-comments-to "/me equivalent of /users/{ids}/comments/{toid}"
 [81]: /docs/favorites-on-users
 [82]: /docs/me-favorites "/me equivalent of /users/{ids}/favorites"
 [83]: /docs/mentions-on-users
 [84]: /docs/me-mentioned "/me equivalent of /users/{ids}/mentioned"
 [85]: /docs/merge-history
 [86]: /docs/me-merge-history "/me equivalent of /users/{ids}/merges"
 [87]: /docs/user-notifications
 [88]: /docs/me-notifications "/me equivalent of /users/{id}/notifications"
 [89]: /docs/user-unread-notifications
 [90]: /docs/me-unread-notifications "/me equivalent of /users/{id}/notifications/unread"
 [91]: /docs/privileges-on-users
 [92]: /docs/me-privileges "/me equivalent of /users/{id}/privileges"
 [93]: /docs/questions-on-users
 [94]: /docs/me-questions "/me equivalent of /users/{ids}/questions"
 [95]: /docs/featured-questions-on-users
 [96]: /docs/me-featured-questions "/me equivalent of /users/{ids}/questions/featured"
 [97]: /docs/no-answer-questions-on-users
 [98]: /docs/me-no-answer-questions "/me equivalent of /users/{ids}/questions/no-answers"
 [99]: /docs/unaccepted-questions-on-users
 [100]: /docs/me-unaccepted-questions "/me equivalent of /users/{ids}/questions/unaccepted"
 [101]: /docs/unanswered-questions-on-users
 [102]: /docs/me-unanswered-questions "/me equivalent of /users/{ids}/questions/unanswered"
 [103]: /docs/reputation-on-users
 [104]: /docs/me-reputation "/me equivalent of /users/{ids}/reputation"
 [105]: /docs/reputation-history
 [106]: /docs/me-reputation-history "/me equivalent of /users/{ids}/reputation-history"
 [107]: /docs/full-reputation-history
 [108]: /docs/me-full-reputation-history "/me equivalent of /users/{ids}/reputation-history/full"
 [109]: /docs/suggested-edits-on-users
 [110]: /docs/me-suggested-edits "/me equivalent of /users/{ids}/suggested-edits"
 [111]: /docs/tags-on-users
 [112]: /docs/me-tags "/me equivalent of /users/{ids}/tags"
 [113]: /docs/top-user-answers-in-tags
 [114]: /docs/me-tags-top-answers "/me equivalent of /users/{id}/tags/{tags}/top-answers"
 [115]: /docs/top-user-questions-in-tags
 [116]: /docs/me-tags-top-questions "/me equivalent of /users/{id}/tags/{tags}/top-questions"
 [117]: /docs/timeline-on-users
 [118]: /docs/me-timeline "/me equivalent of /users/{ids}/timeline"
 [119]: /docs/top-answer-tags-on-users
 [120]: /docs/me-top-answer-tags "/me equivalent of /users/{id}/top-answer-tags"
 [121]: /docs/top-question-tags-on-users
 [122]: /docs/me-top-question-tags "/me equivalent of /users/{id}/top-question-tags"
 [123]: /docs/write-permissions
 [124]: /docs/me-write-permissions "/me equivalent of /users/{ids}/write-permissions"
 [125]: /docs/moderators
 [126]: /docs/elected-moderators
 [127]: /docs/me-inbox "/me equivalent of /users/{id}/inbox"
 [128]: /docs/user-unread-inbox
 [129]: /docs/me-unread-inbox "/me equivalent of /users/{id}/inbox/unread"
 [130]: /docs/invalidate-access-tokens
 [131]: /docs/read-access-tokens
 [132]: /docs/application-de-authenticate
 [133]: /docs/errors
 [134]: /docs/simulate-error
 [135]: /docs/create-filter
 [136]: /docs/read-filter
 [137]: /docs/inbox
 [138]: /docs/inbox-unread
 [139]: /docs/notifications
 [140]: /docs/unread-notifications
 [141]: /docs/sites
 [142]: /docs/associated-users
 [143]: /docs/me-associated-users "/me equivalent of /users/{ids}/associated"
 [144]: /docs/js-lib
 [145]: /docs/numbers
 [146]: /docs/error-handling
 [147]: /docs/user-types
 [148]: /docs/write
 [149]: /docs/change-log
 [150]: http://stackexchange.com/legal/api-terms-of-use

You can also experiment with adding in the Editorial or Dealer API's to add color commentary and availability information.

## DISCLAIMER: 

The sample code described herein is provided on an "as is" basis, without warranty of any kind, to the fullest extent permitted by law. Both Microsoft and I do not warrant or guarantee the individual success developers may have in implementing the sample code on their development platforms or in using their own Web server configurations. 
Microsoft and I do not warrant, guarantee or make any representations regarding the use, results of use, accuracy, timeliness or completeness of any data or information relating to the sample code. Microsoft and I disclaim all warranties, express or implied, and in particular, disclaims all warranties of merchantability, fitness for a particular purpose, and warranties related to the code, or any service or software related thereto. 

Microsoft and I shall not be liable for any direct, indirect or consequential damages or costs of any type arising out of any action taken by you or others related to the sample code.

  [1001]: https://raw.github.com/apimash/StarterKits/master/Windows%20Starter%20Kits/APIMASH_StackExchange_StarterKit/StackExchangeScreenshot.png "StackExchange Starter Kit"
  [1002]: http://api.stackexchange.com "StackExchange"
  [1003]: https://json.codeplex.com/ "JSON.NET"
  [1004]: http://apimash.github.io/StarterKits "APIMASH Starter Kits"
  [1005]: http://htmlagilitypack.codeplex.com/ "HTML Agility Pack"
