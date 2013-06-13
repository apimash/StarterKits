using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace APIMASH_StackExchangeLib
{
    [DataContract]
    public class StackExchangeQuestions
    {
        [DataMember(Name="items")]
        public Question[] Questions { get; set; }

        [DataMember(Name="quota_remaining")]
        public int QuotaRemaining { get; set; }

        [DataMember(Name="quota_max")]
        public int QuotaMax { get; set; }

        [DataMember(Name="has_more")]
        public bool HasMore { get; set; }

        public void Copy(QuestionGroup qg)
        {
            foreach (var qi in Questions.Select( q => new QuestionItem( q )  ))
            {
                qg.Items.Add(qi);
            }
        }
    }

    [DataContract]
    public class Question
    {
        [DataMember(Name = "answer_count")]
        public int AnswerCount { get; set; }

        [DataMember(Name = "accepted_answer_id")]
        public int AcceptedAnswerId { get; set; }

        [DataMember(Name = "question_id")]
        public int Id { get; set; }

        [DataMember(Name = "owner_user_id")]
        public int OwnerId { get; set; }

        [DataMember(Name = "owner")]
        public User Owner { get; set; }

        [DataMember(Name = "owner_display_name")]
        public string OwnerName { get; set; }

        [DataMember(Name = "creation_date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime CreationDate { get; set; }

        [DataMember(Name = "last_edit_date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastEditDate { get; set; }

        [DataMember(Name = "last_activity_date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastActivityDate { get; set; }

        [DataMember(Name = "up_vote_count")]
        public int UpVoteCount { get; set; }

        [DataMember(Name = "down_vote_count")]
        public int DownVoteCount { get; set; }

        [DataMember(Name = "favorite_count")]
        public int FavoriteCount { get; set; }

        [DataMember(Name = "view_count")]
        public int ViewCount { get; set; }

        [DataMember(Name = "score")]
        public int Score { get; set; }

        [DataMember(Name = "community_owned")]
        public bool CommunityOwned { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }

        [DataMember(Name = "link")]
        public string Url { get; set; }

        [DataMember(Name = "comments")]
        public List<Comment> Comments { get; set; }

        [DataMember(Name = "tags")]
        public List<string> Tags { get; set; }

        [DataMember(Name = "answers")]
        public List<Answer> Answers { get; set; }
    }

    [DataContract]
    public class Answer
    {
        [DataMember(Name="answer_id")]
        public int AnswerId { get; set; }

        [DataMember(Name = "accepted")]
        public bool Accepted { get; set; }

        [DataMember(Name = "question_id")]
        public int QuestionId { get; set; }

        [DataMember(Name = "owner_user_id")]
        public int OwnerId { get; set; }

        [DataMember(Name = "owner_display_name")]
        public string OwnerName { get; set; }

        [DataMember(Name = "creation_date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime CreationDate { get; set; }

        [DataMember(Name = "last_edit_date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastEditDate { get; set; }

        [DataMember(Name = "last_activity_date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastActivityDate { get; set; }

        [DataMember(Name = "up_vote_count")]
        public int UpVoteCount { get; set; }

        [DataMember(Name = "down_vote_count")]
        public int DownVoteCount { get; set; }

        [DataMember(Name = "view_count")]
        public int ViewCount { get; set; }

        [DataMember(Name = "community_owned")]
        public bool CommunityOwned { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }

        [DataMember(Name = "comments")]
        public List<Comment> Comments { get; set; }

    }

    [DataContract]
    public class Tag
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }
    }

    [DataContract]
    public class Comment
    {
        [DataMember(Name = "comment_id")]
        public int Id { get; set; }

        [DataMember(Name = "creation_date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime CreationDate { get; set; }

        [DataMember(Name = "owner")]
        public User Owner { get; set; }

        [DataMember(Name = "post_id")]
        public int PostId { get; set; }

        [DataMember(Name = "post_type")]
        public string PostType { get; set; }

        [DataMember(Name = "score")]
        public int Score { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }

        [DataMember(Name = "reply_to_user_id")]
        public int ToID { get; set; }

        [DataMember(Name = "edit_count")]
        public int EditCount { get; set; }

    }

    [DataContract]
    public class User
    {
        [DataMember(Name = "user_id")]
        public int Id { get; set; }

        [DataMember(Name = "user_type")]
        public UserType UserType { get; set; }

        [DataMember(Name = "creation_date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime CreateDate { get; set; }

        [DataMember(Name = "profile_image")]
        public string ProfileImage { get; set; }

        [DataMember(Name = "display_name")]
        public string DisplayName { get; set; }

        [DataMember(Name = "reputation")]
        public int Reputation { get; set; }

        [DataMember(Name = "email_hash")]
        public string EmailHash { get; set; }
    }

    public enum UserType
    {
        anonymous,
        registered,
        unregistered,
        moderator,
        does_not_exist
    }

}
