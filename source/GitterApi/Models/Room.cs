using System;
using System.Linq;
using Newtonsoft.Json;

namespace GitterApi.Models
{
	public class Room
	{
		private const string GitHubTypeOrg = "ORG";
		private const string GitHubTypeRepo = "REPO";
		private const string GitHubTypeOneToOne = "ONETOONE";
		private const string GitHubTypeOrgChannel = "ORG_CHANNEL";
		private const string GitHubTypeRepoChannel = "REPO_CHANNEL";
		private const string GitHubTypeUserChannel = "USER_CHANNEL";

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("topic")]
		public string Topic { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("uri")]
		public string Uri { get; set; }

		[JsonProperty("oneToOne")]
		public bool OneToOne { get; set; }

		[JsonProperty("user")]
		public User User { get; set; }

		[JsonProperty("users")]
		public User[] Users { get; set; }

		[JsonProperty("userCount")]
		public int UserCount { get; set; }

		[JsonProperty("unreadItems")]
		public int UnreadItems { get; set; }

		[JsonProperty("mentions")]
		public int UnreadMentions { get; set; }

		[JsonProperty("lastAccessTime")]
		public DateTime LastAccessTime { get; set; }

		[JsonProperty("favourite")]
		public int FavouriteIndex { get; set; }

		[JsonProperty("lurk")]
		public bool DisabledNotifications { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("githubType")]
		public string GitHubType { get; set; }

		[JsonProperty("tags")]
		public string[] Tags { get; set; }

		[JsonProperty("v")]
		public int Version { get; set; }

		[JsonProperty("groupId")]
		public string GroupId { get; set; }

		[JsonProperty("public")]
		public bool Public { get; set; }

		[JsonProperty("roomMember")]
		public bool RoomMember { get; set; }

		[JsonProperty("exists")]
		public bool Exists { get; set; }

		[JsonIgnore]
		public string GitHubUrl { get { return $"https://github.com{Url}"; } }

		[JsonIgnore]
		public bool Favourite => FavouriteIndex > 0;

		[JsonIgnore]
		public RoomType Type
		{
			get
			{
				switch (GitHubType)
				{
					case GitHubTypeOrg:
						return RoomType.Organisation;
					case GitHubTypeRepo:
						return RoomType.Repository;
					case GitHubTypeOneToOne:
						return RoomType.OneToOne;
					case GitHubTypeOrgChannel:
						return RoomType.OrganisationChannel;
					case GitHubTypeRepoChannel:
						return RoomType.RepositoryChannel;
					case GitHubTypeUserChannel:
						return RoomType.UserChannel;
					default:
						return RoomType.Unknown;
				}
			}
		}

		[JsonIgnore]
		public string Image
		{
			get
			{
				if (User == null)
				{
					var name = Url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
					return $"https://avatars.githubusercontent.com/{name}";
				}
				return User.MediumAvatarUrl;
			}
		}
	}

	public enum RoomType
	{
		Unknown,

		Organisation,
		Repository,
		OneToOne,

		OrganisationChannel,
		RepositoryChannel,
		UserChannel,
	}
}
